# CommandSerializer
C# command-line argument parsing deserialization from attributes.

I know some projects like this already exists but I wanted to put up to date an old 
project I did long time ago. My goal was to make it compile in .Net Core environment.

The reason I created this project was to be able to handle command line arguments like
any serialization class (XmlSerializer for instance).

## Installation
Install from [NuGet](https://www.nuget.org/packages/CommandSerializer.NET).

## Usage

Create a simple Data Class using attributes:

```csharp
[CommandManual(Description = "Write your car manual here")]
public class Car
{
    [Parameter(Action = "automatic", HelpText = "Set the car to automatic")] 
    public bool IsAutomatic { get; set; }

    [Parameter(Action = "colors", Alias = 'c', HelpText = "Set a list of colors (-c red blue ...)")]
    public List<string> Colors { get; set; }

    [Parameter(Alias = 'b', Required = true, HelpText = "Set the brand name")]
    public string Brand { get; set; }

    [PositionalParameter(Name = "NAME", Required = true)]
    public string Name { get; set; }

    [Parameter(Action = "wheels", Alias = 'w', HelpText = "Set the number of wheels your car has.")]
    public int WheelCount { get; set; } = 4;
}
```

Implement it:

```csharp
class TestClass
{
    static void Main(string[] args)
    {
        var result = CommandSerializer<ActionTest>.Parse(args);
        
        Console.WriteLine(result.Name);
    }
}
```

And Test it:

```text
%> ./test --automatic -c silver black -b Toyota Yaris
Yaris
```

### Usage Summary
To generate the Usage summary, just do:
```csharp
var helpText = CommandSerializer<TestClass>.GetHelp();
Console.WriteLine(helpText);
```

and it will show this output:
```text
Usage: CommandSerializer [OPTION]... [NAME]

Mandatory arguments to long options are mandatory for short options too.
      automatic                 Set the car to automatic
  -c, colors                    Set a list of colors (-c red blue ...)
  -b                            [required] Set the brand name
  -w, wheels                    Set the number of wheels your car has.


Write your car manual here
```

## Documentation

### Attributes

When defining a data class, you can decorate (or not) using attibutes. The implementation right
now is really straight forward and it's quite easy to use it.

#### ParameterAttribute

TODO
- They can be required or not.
- case sensitivity
- Alias vs actions and can be omitted. (see example)
- Default must be added to the comment (for now)

#### PositionalParameterAttribute

TODO
- They can be required or not
- Positional parameters will always be read sequentially (from class definition).
  So pay attention if you decide to put them not required.
- cannot have 2 positional parameters as list.

#### CommandManual

This is a really simple attribute where you can add your whole documentation. Since
there is no special format for that, only a string is available and will be printed as-is.

### Parameters

### Supported Types

You can use the following types in your property and it will be converted automatically:

* Basic types (bool, int, double, string, ...)
* Enum
* List<>
* Array
* DateTime (using ``DateTime.Parse``)
* FileInfo
* DirectoryInfo

The default behavior is using ``TypeDescriptor.GetConverter(conversionType)`` to get
the converter and call the ``ConvertFrom``.

### Extending Supported Types

From the existing list, if you want to have specific types that isn't supported (like GUID) 
for instance, you will need to do one of the following:

Set a custom ``TypeConverter`` in the Attribute.

```csharp
[Parameter(Action = "colors", Alias = 'c', Converter = typeof(ColorConverter))]
public Color Color { get; set; }
```

Or add it globally to the ``TypeDescriptor``
```csharp
TypeDescriptor.AddAttributes(
    typeof(Color), 
    new TypeConverterAttribute(typeof(ColorConverter)));
```


### Exceptions

For now, there is only 1 type of exception: ``CommandArgumentException``. Everytime something
unexpected happen, this exception will be thrown with an appropriate message. (@See TODO)

### More

You can check the [UnitTests](ConsoleArgsTests/) for more examples on how to use the CommandSerializer.

## TODO
* Create a .nuget.package
* Add on Git
* Support multiple aliases and actions for the same field? 
  * AllowMultiple? 
  * Don't want to have a multimap
* Create more specific exceptions
* Add the '=' as key-value separator. Right now only ' ' is supported
* Generate Default value in the documentation


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)