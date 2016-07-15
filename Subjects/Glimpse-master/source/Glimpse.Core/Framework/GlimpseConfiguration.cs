using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Glimpse.Core.Extensibility;

namespace Glimpse.Core.Framework
{
    /// <summary>
    /// Contains all configuration required by <see cref="IGlimpseRuntime"/> instances to execute.
    /// </summary>
    public class GlimpseConfiguration : IGlimpseConfiguration
    {
        private static IMessageBroker messageBroker;
        private static Func<IExecutionTimer> timerStrategy;
        private static ILogger logger;
        private ICollection<IClientScript> clientScripts;
        private IResource defaultResource;
        private string endpointBaseUri;
        private IFrameworkProvider frameworkProvider;
        private IHtmlEncoder htmlEncoder;
        private IPersistenceStore persistenceStore;
        private ICollection<IInspector> inspectors;
        private IProxyFactory proxyFactory;
        private ResourceEndpointConfiguration resourceEndpoint;
        private ICollection<IResource> resources;
        private ICollection<IRuntimePolicy> runtimePolicies;
        private ISerializer serializer;
        private ICollection<ITab> tabs;
        private ICollection<IDisplay> displays;
        private static RuntimePolicy defaultRuntimePolicy;
        private static Func<RuntimePolicy> runtimePolicyStrategy;
        private string hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlimpseConfiguration" /> class.
        /// </summary>
        /// <param name="frameworkProvider">The framework provider.</param>
        /// <param name="endpointConfiguration">The resource endpoint configuration.</param>
        /// <param name="clientScripts">The client scripts collection.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="defaultRuntimePolicy">The default runtime policy.</param>
        /// <param name="htmlEncoder">The Html encoder.</param>
        /// <param name="persistenceStore">The persistence store.</param>
        /// <param name="inspectors">The inspectors collection.</param>
        /// <param name="resources">The resources collection.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="tabs">The tabs collection.</param>
        /// <param name="displays">The displays collection.</param>
        /// <param name="runtimePolicies">The runtime policies collection.</param>
        /// <param name="defaultResource">The default resource.</param>
        /// <param name="proxyFactory">The proxy factory.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="endpointBaseUri">The endpoint base Uri.</param>
        /// <param name="timerStrategy">The timer strategy.</param>
        /// <param name="runtimePolicyStrategy">The runtime policy strategy.</param>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if any parameter is <c>null</c>.</exception>
        public GlimpseConfiguration(
            IFrameworkProvider frameworkProvider, 
            ResourceEndpointConfiguration endpointConfiguration,
            ICollection<IClientScript> clientScripts,
            ILogger logger,
            RuntimePolicy defaultRuntimePolicy,
            IHtmlEncoder htmlEncoder,
            IPersistenceStore persistenceStore,
            ICollection<IInspector> inspectors,
            ICollection<IResource> resources,
            ISerializer serializer,
            ICollection<ITab> tabs,
            ICollection<IDisplay> displays,
            ICollection<IRuntimePolicy> runtimePolicies,
            IResource defaultResource,
            IProxyFactory proxyFactory,
            IMessageBroker messageBroker,
            string endpointBaseUri,
            Func<IExecutionTimer> timerStrategy,
            Func<RuntimePolicy> runtimePolicyStrategy)
        {
            if (frameworkProvider == null)
            {
                throw new ArgumentNullException("frameworkProvider");
            }

            if (endpointConfiguration == null)
            {
                throw new ArgumentNullException("endpointConfiguration");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (htmlEncoder == null)
            {
                throw new ArgumentNullException("htmlEncoder");
            }

            if (persistenceStore == null)
            {
                throw new ArgumentNullException("persistenceStore");
            }

            if (clientScripts == null)
            {
                throw new ArgumentNullException("clientScripts");
            }

            if (resources == null)
            {
                throw new ArgumentNullException("inspectors");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (tabs == null)
            {
                throw new ArgumentNullException("tabs");
            }

            if (displays == null)
            {
                throw new ArgumentNullException("displays");
            }

            if (runtimePolicies == null)
            {
                throw new ArgumentNullException("runtimePolicies");
            }

            if (defaultResource == null)
            {
                throw new ArgumentNullException("defaultResource");
            }

            if (proxyFactory == null)
            {
                throw new ArgumentNullException("proxyFactory");
            }

            if (messageBroker == null)
            {
                throw new ArgumentNullException("messageBroker");
            }

            if (endpointBaseUri == null)
            {
                throw new ArgumentNullException("endpointBaseUri");
            }

            if (timerStrategy == null)
            {
                throw new ArgumentNullException("timerStrategy");
            }

            if (runtimePolicyStrategy == null)
            {
                throw new ArgumentNullException("runtimePolicyStrategy");
            }

            Logger = logger;
            ClientScripts = clientScripts;
            FrameworkProvider = frameworkProvider;
            HtmlEncoder = htmlEncoder;
            PersistenceStore = persistenceStore;
            Inspectors = inspectors;
            ResourceEndpoint = endpointConfiguration;
            Resources = resources;
            Serializer = serializer;
            Tabs = tabs;
            Displays = displays;
            RuntimePolicies = runtimePolicies;
            DefaultRuntimePolicy = defaultRuntimePolicy;
            DefaultResource = defaultResource;
            ProxyFactory = proxyFactory;
            MessageBroker = messageBroker;
            EndpointBaseUri = endpointBaseUri;
            TimerStrategy = timerStrategy;
            RuntimePolicyStrategy = runtimePolicyStrategy;
        }

        /// <summary>
        /// Gets or sets the client scripts collection.
        /// </summary>
        /// <value>
        /// The client scripts.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ICollection<IClientScript> ClientScripts
        {
            get
            {
                return clientScripts;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                clientScripts = value;
            }
        }

        /// <summary>
        /// Gets or sets the default <see cref="IResource"/> to execute.
        /// </summary>
        /// <value>
        /// The default resource.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IResource DefaultResource
        {
            get
            {
                return defaultResource;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                defaultResource = value;
            }
        }

        /// <summary>
        /// Gets or sets the default runtime policy.
        /// </summary>
        /// <value>
        /// The default runtime policy.
        /// </value>
        public RuntimePolicy DefaultRuntimePolicy
        {
            get
            {
                return defaultRuntimePolicy;
            }

            set
            {
                defaultRuntimePolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint base URI.
        /// </summary>
        /// <value>
        /// The endpoint base URI.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public string EndpointBaseUri
        {
            get
            {
                return endpointBaseUri;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                endpointBaseUri = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IFrameworkProvider"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="IFrameworkProvider"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IFrameworkProvider FrameworkProvider
        {
            get
            {
                return frameworkProvider;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                frameworkProvider = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IHtmlEncoder"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="IHtmlEncoder"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IHtmlEncoder HtmlEncoder
        {
            get
            {
                return htmlEncoder;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                htmlEncoder = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ILogger"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="ILogger"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ILogger Logger
        {
            get
            {
                return logger;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                logger = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IMessageBroker"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="IMessageBroker"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IMessageBroker MessageBroker
        {
            get
            {
                return messageBroker;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                messageBroker = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IPersistenceStore"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="IPersistenceStore"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IPersistenceStore PersistenceStore
        {
            get
            {
                return persistenceStore;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                persistenceStore = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="IInspector"/>.
        /// </summary>
        /// <value>
        /// The configured collection of <see cref="IInspector"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ICollection<IInspector> Inspectors
        {
            get
            {
                return inspectors;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                inspectors = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IProxyFactory"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="IProxyFactory"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public IProxyFactory ProxyFactory
        {
            get
            {
                return proxyFactory;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                proxyFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ResourceEndpointConfiguration"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="ResourceEndpointConfiguration"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ResourceEndpointConfiguration ResourceEndpoint
        {
            get
            {
                return resourceEndpoint;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                resourceEndpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="IResource"/>.
        /// </summary>
        /// <value>
        /// The configured collection of <see cref="IResource"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ICollection<IResource> Resources
        {
            get
            {
                return resources;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                resources = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="IRuntimePolicy"/>.
        /// </summary>
        /// <value>
        /// The configured collection of <see cref="IRuntimePolicy"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ICollection<IRuntimePolicy> RuntimePolicies
        {
            get
            {
                return runtimePolicies;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                runtimePolicies = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RuntimePolicy"/> strategy.
        /// </summary>
        /// <value>
        /// The configured <see cref="RuntimePolicy"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public Func<RuntimePolicy> RuntimePolicyStrategy
        {
            get
            {
                return runtimePolicyStrategy;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                runtimePolicyStrategy = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISerializer"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="ISerializer"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ISerializer Serializer
        {
            get
            {
                return serializer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                serializer = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="ITab"/>.
        /// </summary>
        /// <value>
        /// The configured <see cref="ITab"/>.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public ICollection<ITab> Tabs
        {
            get
            {
                return tabs;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                tabs = value;
            }
        }

        public ICollection<IDisplay> Displays
        {
            get
            {
                return displays;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                displays = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IExecutionTimer"/> strategy.
        /// </summary>
        /// <value>
        /// The configured <see cref="IExecutionTimer"/> strategy.
        /// </value>
        /// <exception cref="System.ArgumentNullException">An exception is thrown if the value is set to <c>null</c>.</exception>
        public Func<IExecutionTimer> TimerStrategy 
        { 
            get
            {
                return timerStrategy;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                timerStrategy = value;
            }
        }

        public string Hash
        {
            get
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    return hash;
                }

                var configuredTypes = new List<Type> { GetType() };
                configuredTypes.AddRange(Tabs.Select(tab => tab.GetType()).OrderBy(type => type.Name));
                configuredTypes.AddRange(Inspectors.Select(inspector => inspector.GetType()).OrderBy(type => type.Name));
                configuredTypes.AddRange(Resources.Select(resource => resource.GetType()).OrderBy(type => type.Name));
                configuredTypes.AddRange(ClientScripts.Select(clientScript => clientScript.GetType()).OrderBy(type => type.Name));
                configuredTypes.AddRange(RuntimePolicies.Select(policy => policy.GetType()).OrderBy(type => type.Name));

                var crc32 = new Crc32();
                var sb = new StringBuilder();
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, configuredTypes);
                    memoryStream.Position = 0;

                    var computeHash = crc32.ComputeHash(memoryStream);

                    foreach (var b in computeHash)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                }

                hash = sb.ToString();
                return hash;
            }

            set
            {
                hash = value;
            }
        }

        [Obsolete("HACK: To support TraceListener with TraceSource via web.config")]
        public static ILogger GetLogger()
        {
            return logger;
        }

        [Obsolete("HACK: To support TraceListener with TraceSource via web.config")]
        public static Func<IExecutionTimer> GetConfiguredTimerStrategy()
        {
            return () =>
            {
                try
                {
                    return timerStrategy();
                }
                catch
                {
                    // Avoid exception being thrown from threads without access to request store
                    return null;
                }
            }; 
        }

        [Obsolete("HACK: To support TraceListener with TraceSource via web.config")]
        public static IMessageBroker GetConfiguredMessageBroker()
        {
            return messageBroker;
        }

        [Obsolete("HACK: To prevent unnecessary wrapping of SQL connections, commands etc")]
        public static RuntimePolicy GetDefaultRuntimePolicy()
        {
            return defaultRuntimePolicy;
        }

        [Obsolete("HACK: To prevent unnecessary wrapping of SQL connections, commands etc")]
        public static Func<RuntimePolicy> GetRuntimePolicyStategy()
        {
            return runtimePolicyStrategy;
        }
    }
}
