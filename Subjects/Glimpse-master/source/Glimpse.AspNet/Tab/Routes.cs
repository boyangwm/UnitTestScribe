using System.Collections.Generic;
using System.Linq;
using Glimpse.AspNet.AlternateType;
using Glimpse.AspNet.Extensibility;
using Glimpse.AspNet.Model;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;
using MvcRoute = System.Web.Routing.Route;
using MvcRouteBase = System.Web.Routing.RouteBase;
using MvcRouteValueDictionary = System.Web.Routing.RouteValueDictionary;
using System.Reflection;
using Glimpse.AspNet.Message;

namespace Glimpse.AspNet.Tab
{
    public class Routes : AspNetTab, IDocumentation, ITabSetup, ITabLayout, IKey
    {
        private static readonly object Layout = TabLayout.Create()
                .Row(r =>
                {
                    r.Cell(0).WidthInPixels(100);
                    r.Cell(1).AsKey();
                    r.Cell(2);
                    r.Cell(3).WidthInPercent(20).SetLayout(TabLayout.Create().Row(x => 
                        {
                            x.Cell("{{0}} ({{1}})").WidthInPercent(45); 
                            x.Cell(2);
                        }));
                    r.Cell(4).WidthInPercent(35).SetLayout(TabLayout.Create().Row(x =>
                        {
                            x.Cell(0).WidthInPercent(30);
                            x.Cell(1);
                            x.Cell(2).WidthInPercent(30);
                        }));
                    r.Cell(5).WidthInPercent(15).SetLayout(TabLayout.Create().Row(x =>
                        {
                            x.Cell(0).WidthInPercent(45);
                            x.Cell(1).WidthInPercent(55); 
                        }));
                    r.Cell(6).WidthInPixels(100).Suffix(" ms").Class("mono").AlignRight();
                }).Build();

        public override string Name
        {
            get { return "Routes"; }
        }

        public string Key
        {
            get { return "glimpse_routes"; }
        }

        public string DocumentationUri
        {
            get { return "http://getGlimpse.com/Help/Routes-Tab"; }
        }

        public object GetLayout()
        {
            return Layout;
        }

        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<ProcessConstraintMessage>();
            context.PersistMessages<RouteDataMessage>();
        }

        public override object GetData(ITabContext context)
        {
            var routeMessages = ProcessMessages(context.GetMessages<RouteDataMessage>());
            var constraintMessages = ProcessMessages(context.GetMessages<ProcessConstraintMessage>());

            var result = new List<RouteModel>();
            
            using (System.Web.Routing.RouteTable.Routes.GetReadLock())
            {
                foreach (var routeBase in System.Web.Routing.RouteTable.Routes)
                {
                    if (routeBase.GetType().ToString() == "System.Web.Mvc.Routing.LinkGenerationRoute")
                    {
                        continue;
                    }

                    if (routeBase.GetType().ToString() == "System.Web.Mvc.Routing.RouteCollectionRoute")
                    {
                        // This catches any routing that has been defined using Attribute Based Routing
                        // System.Web.Http.Routing.RouteCollectionRoute is a collection of HttpRoutes

                        var subRoutes = routeBase.GetType().GetField("_subRoutes", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(routeBase);
                        var _routes = (IList<System.Web.Routing.Route>)subRoutes.GetType().GetField("_routes", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(subRoutes);

                        for (var i = 0; i < _routes.Count; i++)
                        {
                            var routeModel = GetRouteModelForRoute(context, _routes[i], routeMessages, constraintMessages);
                            result.Add(routeModel);
                        }
                    }
                    else
                    {
                        var routeModel = GetRouteModelForRoute(context, routeBase, routeMessages, constraintMessages);

                        result.Add(routeModel);
                    }
                }
            }

            return result;
        }

        private static TSource SafeFirstOrDefault<TSource>(IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return default(TSource);
            }

            return source.FirstOrDefault();
        }

        private Dictionary<int, List<RouteDataMessage>> ProcessMessages(IEnumerable<RouteDataMessage> messages)
        { 
            if (messages == null)
            {
                return new Dictionary<int, List<RouteDataMessage>>();
            }

            return messages.GroupBy(x => x.RouteHashCode).ToDictionary(x => x.Key, x => x.ToList());
        }

        private Dictionary<int, Dictionary<int, List<ProcessConstraintMessage>>> ProcessMessages(IEnumerable<ProcessConstraintMessage> messages)
        {
            if (messages == null)
            {
                return new Dictionary<int, Dictionary<int, List<ProcessConstraintMessage>>>();
            }

            return messages.GroupBy(x => x.RouteHashCode).ToDictionary(x => x.Key, x => x.ToList().GroupBy(y => y.ConstraintHashCode).ToDictionary(y => y.Key, y => y.ToList()));
        }

        private RouteModel GetRouteModelForRoute(ITabContext context, MvcRouteBase routeBase, Dictionary<int, List<RouteDataMessage>> routeMessages, Dictionary<int, Dictionary<int, List<ProcessConstraintMessage>>> constraintMessages)
        {
            var routeModel = new RouteModel();

            var routeMessage = SafeFirstOrDefault(routeMessages.GetValueOrDefault(routeBase.GetHashCode()));
            if (routeMessage != null)
            {
                routeModel.Duration = routeMessage.Duration; 
                routeModel.IsMatch = routeMessage.IsMatch;
            }

            var route = routeBase as MvcRoute;
            if (route != null)
            {
                routeModel.Area = (route.DataTokens != null && route.DataTokens.ContainsKey("area")) ? route.DataTokens["area"].ToString() : null;
                routeModel.Url = route.Url;
                routeModel.RouteData = ProcessRouteData(route.Defaults, routeMessage);
                routeModel.Constraints = ProcessConstraints(context, route, constraintMessages);
                routeModel.DataTokens = ProcessDataTokens(route.DataTokens);
            }
            else
            {
                routeModel.Url = routeBase.ToString();
            }

            var routeName = routeBase as IRouteNameMixin;
            if (routeName != null)
            {
                routeModel.Name = routeName.Name;
            }

            return routeModel;
        }

        private IEnumerable<RouteDataItemModel> ProcessRouteData(MvcRouteValueDictionary dataDefaults, RouteDataMessage routeMessage)
        {
            if (dataDefaults == null || dataDefaults.Count == 0)
            {
                return null;
            }

            var routeData = new List<RouteDataItemModel>();
            foreach (var dataDefault in dataDefaults)
            {
                var routeDataItemModel = new RouteDataItemModel();
                routeDataItemModel.PlaceHolder = dataDefault.Key;
                routeDataItemModel.DefaultValue = dataDefault.Value;

                if (routeMessage != null && routeMessage.Values != null)
                {
                    routeDataItemModel.ActualValue = routeMessage.Values[dataDefault.Key];
                }

                routeData.Add(routeDataItemModel);
            }

            return routeData;
        }

        private IEnumerable<RouteConstraintModel> ProcessConstraints(ITabContext context, MvcRoute route, Dictionary<int, Dictionary<int, List<ProcessConstraintMessage>>> constraintMessages)
        {
            if (route.Constraints == null || route.Constraints.Count == 0)
            {
                return null;
            }
             
            var counstraintRouteMessages = constraintMessages.GetValueOrDefault(route.GetHashCode()); 

            var result = new List<RouteConstraintModel>();
            foreach (var constraint in route.Constraints)
            {
                var model = new RouteConstraintModel();
                model.ParameterName = constraint.Key;
                model.Constraint = constraint.Value.ToString();

                if (counstraintRouteMessages != null)
                {
                    var counstraintMessage = SafeFirstOrDefault(counstraintRouteMessages.GetValueOrDefault(constraint.Value.GetHashCode()));
                    model.IsMatch = false;
                    
                    if (counstraintMessage != null)
                    {
                        model.IsMatch = counstraintMessage.IsMatch;
                    }
                }

                result.Add(model);
            }

            return result;
        }


        private IDictionary<string, object> ProcessDataTokens(IDictionary<string, object> dataTokens)
        {
            return dataTokens != null && dataTokens.Count > 0 ? dataTokens : null;
        }
    }
}
