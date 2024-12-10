using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

sealed class Node : IEnumerable<Node>
{
    enum _ { Value, Array, Object }

    readonly XElement Element; readonly _ Type;

    static readonly NotSupportedException Exception = new();

    Node(XElement _) => Type = (Element = _).Attribute("type").Value[0] switch
    {
        'a' => Node._.Array,
        'o' => Node._.Object,
        _ => Node._.Value
    };

    internal Node this[string _] => Type is Node._.Object ? new(Element.Elements().First(value => Key(value) == _)) : throw Exception;

    internal Node this[int _] => Type is Node._.Array ? new(Element.Elements().ElementAt(_)) : throw Exception;

    internal string Value => Type is @_.Value ? Element.Value : throw Exception;

    static string Key(XElement _) => string.IsNullOrEmpty(_.Name.NamespaceName) ? _.Name.LocalName : _.Attribute("item").Value;

    internal static Node Get(Stream _)
    {
        using var reader = JsonReaderWriterFactory.CreateJsonReader(_, XmlDictionaryReaderQuotas.Max);
        return new(XElement.Load(reader));
    }

    internal static async Task<Node> GetAsync(Stream _) => await Task.Run(() => Get(_));

    public IEnumerator<Node> GetEnumerator() => Type is not @_.Value ? Element.Elements().Select(_ => new Node(_)).GetEnumerator() : throw Exception;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}