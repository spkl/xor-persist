xor-persist
===========
XorPersist is an easy to use XML serialization library for .NET.
With XorPersist, you can save your object graph to an XML file and restore it again later.

**Table of Contents**

- [Features](#features)
	- [Primitive data type support](#primitive-data-type-support)
	- [Other supported .NET classes](#other-supported-net-classes)
	- [Nullables](#nullables)
	- [Collection support](#collection-support)
- [Usage](#usage)
	- [Getting started](#getting-started)
		- [Preparing the data model](#preparing-the-data-model)
		- [Saving and restoring](#saving-and-restoring)
	- [Exceptions](#exceptions)
		- [ClassAttributeMissingException](#classattributemissingexception)
		- [CouldNotResolveReferenceException](#couldnotresolvereferenceexception)
		- [CtorMissingException](#ctormissingexception)
		- [DuplicateXorClassNameException](#duplicatexorclassnameexception)
		- [PropertyTypeNotSupportedException](#propertytypenotsupportedexception)
        - [InvalidXorAttributeNameException](#invalidxorattributenameexception)
		- [Other Exceptions](#other-exceptions)
		- [SchemaValidationException](#schemavalidationexception)
	- [Interface usage](#interface-usage)
	- [Custom serialization](#custom-serialization)
	- [Private or protected parameterless constructor](#private-or-protected-parameterless-constructor)
- [Limitations](#limitations)

## Features
- Native support for most primitive data types and major .NET data and collection classes.
- Class inheritance and interfaces are fully supported.
- Data types without native support can be used by specifying custom serialization and deserialization.
- Customizable and extensible saving and loading of classes and properties.
- XSD schema validation for all input files.
- Classes without a public constructor are supported.

### Primitive data type support
Supports `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `decimal`, `bool`, `char` and `string`.

### Other supported .NET classes
- `System.Guid`
- `System.DateTime`
- `System.TimeSpan`

### Nullables
All types mentioned in the previous two sections are also supported when used as T for `System.Nullable<T>`.

### Collection support
- `System.Collections.ArrayList`
- `System.Collections.ICollection`
- `System.Collections.IList`
- `System.Collections.Generic.HashSet<T>`
- `System.Collections.Generic.List<T>`
- `System.Collections.Generic.SortedSet<T>`
- `System.Collections.Generic.ICollection<T>`
- `System.Collections.Generic.IList<T>`
- `System.Collections.Generic.ISet<T>`
- `System.Collections.Queue`
- `System.Collections.Stack`
- `System.Collections.Generic.LinkedList<T>`
- `System.Collections.Generic.Queue<T>`
- `System.Collections.Generic.Stack<T>`
- Dictionaries can be used by storing the keys and values as two separate collections.


## Usage
To use XorPersist, your class model must represent a [tree](http://en.wikipedia.org/wiki/Tree_%28data_structure%29). You need a root node that has no parent node and contains (by proxy) all other nodes.
There can be diagonal references between nodes, but every node (aside from the root) must have exactly one parent node.

### Getting started
#### Preparing the data model
Derive your class from [`XorObject`](src/XorPersist/XorObject.cs) and decorate it with an [`XorClassAttribute`](src/XorPersist/Attributes/XorClassAttribute.cs):

```csharp
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;

[XorClass(typeof(Company))]
class Company : XorObject { }
```

If you cannot directly derive your class from XorObject, you can also derive a base class of your class from XorObject.

The parameter of the `XorClass` attribute defines which name your class is going to have in the XML file. You can specify a string or a `System.Type`:
If you specify a type, the FullName of the type will be used. In migration scenarios between different versions of a data model, 
it can be useful to specify a string instead of a type, because the type name might change.

In the next step, decorate all properties and fields you want to save with an [`XorPropertyAttribute`](src/XorPersist/Attributes/XorPropertyAttribute.cs)
and define names for them:

```csharp
[XorClass(typeof(Company))]
class Company : XorObject
{
    [XorProperty("Name")]
    public string Name { get; set; }

    [XorProperty("YearEstablished")]
    public int YearEstablished { get; set; }
}
```

All properties and fields that are decorated with an `XorProperty` attribute will be saved. Obviously, a class can also contain other classes.
If your class is the direct parent of another class, use the `XorProperty` attribute:

```csharp
[XorProperty("Headquarter")]
public Building Headquarter { get; set; }
```

When the property does not hold a single instance or value, but is a collection, define the [`XorMultiplicity`](src/XorPersist/Attributes/XorMultiplicity.cs) List value.
A list of supported collection classes can be found in the *Collection support* section of this document.

```csharp
[XorProperty("Employees", XorMultiplicity.List)]
public List<Employee> Employees { get; set; }
```

If your class only references another class but does not own it, or owns it, but in a different relationship (property),
use the [`XorReference`](src/XorPersist/Attributes/XorReferenceAttribute.cs) attribute. This will let XorPersist know,
that this is only a reference to the object, and not the parent-child relationship.

```csharp
[XorReference("CEO")]
public Employee CEO { get; set; }
```

Like `XorProperty`, the `XorReference` attribute also supports collections:

```csharp
[XorReference("BoardMembers", XorMultiplicity.List)
public List<Employee> BoardMembers { get; set; }
```

When you apply `XorProperty` or `XorReference` to a property, you don't need to specify the name. The name of the property will automatically be used.
This feature uses the [CallerMemberNameAttribute](https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute(v=vs.110).aspx) that is available since .NET 4.5.

```csharp
[XorProperty(XorMultiplicity.List)]
public List<Employee> Employees { get; set; }

[XorReference]
public Employee CEO { get; set; }
```

You still need to specify a name for fields. Not specifying a name for a field will result in an exception ([InvalidXorAttributeNameException](#invalidxorattributenameexception)) during runtime:

```csharp
// This will not work!
[XorProperty]
public string TestField;
```

#### Saving and restoring
When your data model is prepared for use with XorPersist, you can use the [`XorController`](src/XorPersist/XorController.cs) to load and restore the data.

```csharp
// Saving:
Company myCompany = new Company();
XorController.Get().Save(myCompany, @"path/to/save.to");

// Restoring:
myCompany = XorController.Get().Load<Company>(@"path/to/load.from");
```

If an error occurs while saving or loading, an Exception will be raised. This can be an [`XorPersistException`](src/XorPersist/Exceptions/XorPersistException.cs)
when your data model does not provide all required information or contains another error, or other .NET exceptions.
See the *Exceptions* chapter in this document for more information.

### Exceptions
XorPersist uses the base class `XorPersistException` for all custom exception types.

##### ClassAttributeMissingException
This Exception is raised when XorPersist discovers a class that has no `XorClass` attribute. The Exception text contains the affected class.

##### CouldNotResolveReferenceException
This Exception is raised when the target of stored reference cannot be found while loading a model. This happens if there are classes in the model that are only
referenced (`XorReference`), so they have no owner. Every object in the model has to live in a `XorProperty` property or field of another object.

##### CtorMissingException
This Exception is raised when XorPersist does not find a public parameterless constructor. For more information,
see chapter *Private or protected parameterless constructor* in this document.

##### DuplicateXorClassNameException
This Exception is raised when two different classes both define the same name in their `XorClass` attribute.

##### PropertyTypeNotSupportedException
This Exception is raised when XorPersist discovers a field or property with an unsupported data type. You can still use the data type with XorPersist,
but you have to declare a 'conversion' property that uses a supported data type. In the following example, a custom string class property is serialized by
a different property, that is only used by XorPersist.

```csharp
[XorClass(typeof(CustomConversionExample))]
class CustomConversionExample : XorObject
{
    // The type of this property is not supported...
    public CustomString Description { get; set; }

    // ... so the following property will perform
    // the conversion to a supported data type.
    [XorProperty("Description")]
    private string _Description
    {
        get { return this.Description.ToString(); }
        set { this.Description = new CustomString(value); }
    }
}
```

##### InvalidXorAttributeNameException
This Exception is raised, when an invalid name is passed to an `XorAttribute` constructor. Invalid names are:
- `null`
- The empty string ("").
- A whitespace-only string ("  ").

The InvalidXorAttributeNameException is also raised, when you apply an `XorProperty` or `XorReference` attribute to
a field (not a property) and don't specify a name.


##### Other Exceptions
This chapter lists a selection of system exceptions that may be raised by XorPersist.

- System.Xml.XmlException - This Exception may be raised when trying to load a model from a file that is not an XML file.
- System.IO.FileNotFoundException - This Exception may be raised when trying to load a model from a file path that does not exist.
- System.IO.IOException - This Exception may be raised when trying to load/save from/to a file that cannot be read/written.
  The access rights of the current user might be insufficient or the file might be opened in a different application.

##### SchemaValidationException
This Exception is raised when trying to load a model from a file that is not valid according to the [XorPersist XSD schema](src/XorPersist/Schema/XorPersist.xsd).

### Interface usage
You do not have to declare a class that inherits from XorObject as a property type. You can also use an interface. XorPersist will detect the actual type used.

```csharp
interface IVehicle { }

[XorClass(typeof(Car))]
class Car : XorObject, IVehicle { }

[XorClass(typeof(Truck))]
class Truck : XorObject, IVehicle { }

[XorClass(typeof(ParkingSpot))]
class ParkingSpot
{
    [XorProperty("ParkedVehicle")]
    IVehicle ParkedVehicle { get; set; }
}
```

### Custom serialization
You can perform custom actions before saving the model and after restoring the model. Override the appropriate methods of XorObject.
`XorInitialize` is called after the whole model was restored. `XorFinish` is called immediately before the object is serialized.

```csharp
[XorClass(typeof(MyClass))]
class MyClass : XorObject
{
    protected override void XorInitialize()
    {
        // ...
    }

    protected override void XorFinish()
    {
        // ...
    }
}
```


### Private or protected parameterless constructor
A public parameterless constructor is needed to instantiate an unknown class. If you don't want to provide that in your data model,
you can also define the static `_XorCreate()` method for XorPersist to use. This method can also be private.

```csharp
[XorClass(typeof(MyClass))]
class MyClass : XorObject
{
    private MyClass() { }

    private static object _XorCreate()
    {
        return new MyClass();
    }
}
```

## Limitations
- Dictionaries are not seamlessly supported.
  - Workaround: Serialize the key and value collections separately.
- Generic types might not be fully supported.



*Table of Contents generated with [DocToc](http://doctoc.herokuapp.com/)*
