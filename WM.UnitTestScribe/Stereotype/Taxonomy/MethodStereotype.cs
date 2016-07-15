using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.UnitTestScribe.Stereotype.Taxonomy {

    public class MethodStereotype : Stereotype{
        public static readonly MethodStereotype Get = new MethodStereotype(0, Category.Accessor, Subcategory.Get);
        public static readonly MethodStereotype Predicate = new MethodStereotype(1, Category.Accessor, Subcategory.Predicate);
        public static readonly MethodStereotype Property = new MethodStereotype(2, Category.Accessor, Subcategory.Property);
        public static readonly MethodStereotype Set = new MethodStereotype(3, Category.Mutator, Subcategory.SET);
        public static readonly MethodStereotype Command = new MethodStereotype(4, Category.Mutator, Subcategory.Command);
        public static readonly MethodStereotype NonVoidCommand = new MethodStereotype(5, Category.Mutator, Subcategory.Non_void_command);
        public static readonly MethodStereotype Constructor = new MethodStereotype(6, Category.Creational, Subcategory.Constructor);
        public static readonly MethodStereotype CopyConstructor = new MethodStereotype(7, Category.Creational, Subcategory.Copy_constructor);
        public static readonly MethodStereotype Destructor = new MethodStereotype(8, Category.Creational, Subcategory.Destructor);
        public static readonly MethodStereotype Factory = new MethodStereotype(9, Category.Creational, Subcategory.Factory);
        public static readonly MethodStereotype Collaborator = new MethodStereotype(10, Category.Collaborational, Subcategory.Collaborator);
        public static readonly MethodStereotype Controller = new MethodStereotype(10, Category.Collaborational, Subcategory.Controller);
        public static readonly MethodStereotype LocalController = new MethodStereotype(10, Category.Collaborational, Subcategory.Local_controller); 
        public static readonly MethodStereotype Empty = new MethodStereotype(13, Category.Degenerate, Subcategory.Empty);
        public static readonly MethodStereotype Abstract = new MethodStereotype(14, Category.Degenerate, Subcategory.Abstract);
        public static readonly MethodStereotype Unclassified = new MethodStereotype(15, Category.Degenerate, Subcategory.Unclassified);

        public int Id { get; private set; }
        public Category Category { get; private set; }
        public Subcategory Subcategory { get; private set; }

        /// <summary>
        /// Returns method StereotypeRole  
        /// </summary>
        /// <returns></returns>
        public override StereotypeRole GetStereotypeRole() {
            return StereotypeRole.Method;
        }


        /// <summary>
        /// Gets the name of this method stereotype
        /// </summary>
        /// <returns></returns>
        public override string GetStereotypeName() {
            return this.Subcategory.Name;
        }


        public MethodStereotype(int id, Category cat, Subcategory subcat) {
            this.Id = id;
            this.Category = cat;
            this.Subcategory = subcat;
        }

        /// <summary>
        /// returns all the method stereotypes
        /// </summary>
        public static IEnumerable<MethodStereotype> Values {
            get {
                yield return Get;
                yield return Predicate;
                yield return Property;
                yield return Command;
                yield return NonVoidCommand;
                yield return Constructor;
                yield return CopyConstructor;
                yield return Destructor;
                yield return Factory;
                yield return Collaborator;
                yield return Empty;
                yield return Abstract;
                yield return Unclassified;
            }
        }

    }


    /// <summary>
    /// Method stereotype categories
    /// </summary>
    public class Category {
        public static readonly Category Accessor = new Category("Accessor", 0);
        public static readonly Category Mutator = new Category("Mutator", 1);
        public static readonly Category Collaborational = new Category("Collaborational", 2);
        public static readonly Category Creational = new Category("Creational", 3);
        public static readonly Category Degenerate = new Category("Degenerate", 4);


        private readonly string name;
        private readonly int id;

        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// ID of the category
        /// </summary>
        public int Id { get { return id; } }

        /// <summary>
        /// Get all method categories
        /// </summary>
        public static IEnumerable<Category> Values {
            get {
                yield return Accessor;
                yield return Mutator;
                yield return Collaborational;
                yield return Creational;
                yield return Degenerate;
            }
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        Category(string name, int id) {
            this.name = name;
            this.id = id;
        }
    }



    public class Subcategory {

        public static readonly Subcategory Get = new Subcategory("Get", 0, "Returns a local field directly");
        public static readonly Subcategory Predicate = new Subcategory("Predicate", 1, "Returns a Boolean value that is not a local field");
        public static readonly Subcategory Property = new Subcategory("Property", 2, "Returns information about local fields");
        public static readonly Subcategory SET = new Subcategory("SET", 3, "Changes only one local field");
        public static readonly Subcategory Command = new Subcategory("Command", 4, "Changes more than one local fields");
        public static readonly Subcategory Non_void_command = new Subcategory("Non_void_command", 5, "Command whose return type is not void or Boolean");
        public static readonly Subcategory Constructor = new Subcategory("Constructor", 6, "Invoked when creating an object");
        public static readonly Subcategory Copy_constructor = new Subcategory("Copy_constructor", 7, "Creates a new object as a copy of the existing one");
        public static readonly Subcategory Destructor = new Subcategory("Destructor", 8, "Performs any necessary cleanups before the object is destroyed");
        public static readonly Subcategory Factory = new Subcategory("Factory", 9, "Instantiates an object and returns it");
        public static readonly Subcategory Collaborator = new Subcategory("Collaborator", 10, "Connects one object with other type of objects");
        public static readonly Subcategory Controller = new Subcategory("Controller", 11, "Provides control logic by invoking only external methods");
        public static readonly Subcategory Local_controller = new Subcategory("Local_controller", 12, "Provides control logic by invoking only local methods"); 
        public static readonly Subcategory Empty = new Subcategory("Empty", 13, "Has no statements");
        public static readonly Subcategory Abstract = new Subcategory("Abstract", 14, "Has no body");
        public static readonly Subcategory Unclassified = new Subcategory("Incidental", 15, "Any other case");

        /* More categories in ChangeScribe
        VOID_ACCESSOR   //Returns information about local fields through the parameters"), 
        */

        /// <summary>
        /// Get all method subcategories
        /// </summary>
        public static IEnumerable<Subcategory> Values {
            get {
                yield return Get;
                yield return Predicate;
                yield return Property;
                yield return Command;
                yield return Non_void_command;
                yield return Constructor;
                yield return Copy_constructor;
                yield return Destructor;
                yield return Factory;
                yield return Collaborator;
                yield return Empty;
                yield return Abstract;
                yield return Unclassified;
            }
        }


        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }

        Subcategory(string name, int id, string desc) {
            this.Name = name;
            this.Id = id;
            this.Description = desc;
        }
    }
}
