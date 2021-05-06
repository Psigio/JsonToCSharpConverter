# JsonToCSharpConverter

A simple .net-core [Avalonia UI](https://github.com/AvaloniaUI/Avalonia) application that allows you to convert a Json object, such as:

```
{ "name" : "one",
  "value": 1,
  "sub": {
      "anotherKey": false,
      "others": [1, 2]
  }
}
```

into a C# anonymous object snippet:
```
var a = new 
{
    name = "one",
    value = 1,
    sub = new 
    {
        anotherKey = false,
        others = new [] { 1, 2 }
    }
};
```

> Note: This is a work in progress, so is almost certainly buggy.  Use at your own risk!

TODO - clean up code, unit tests, improve formatting of C# output etc
