using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

readonly struct Json : IEnumerable<Json>
{
    enum _ { Value, Array, Object }

    readonly XElement Element;

    readonly _ Type;

    Json(XElement value) => Type = (Element = value).Attribute("type").Value[0] switch { 'a' => _.Array, 'o' => _.Object, _ => _.Value };

    internal Json this[string key] => Type is @_.Object
    ? new(Element.Elements().First(_ => (string.IsNullOrEmpty(_.Name.NamespaceName) ? _.Name.LocalName : _.Attribute("item").Value) == key))
    : throw new NotSupportedException();

    internal Json this[int index] => Type is @_.Array
    ? new(Element.Elements().ElementAt(index))
    : throw new NotSupportedException();

    internal string Value => Type is @_.Value
    ? Element.Value
    : throw new NotSupportedException();

    internal static Json Parse(Stream stream)
    {
        using var reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
        return new(XElement.Load(reader));
    }

    internal static async Task<Json> ParseAsync(Stream stream) => await Task.Run(() => Parse(stream));

    public IEnumerator<Json> GetEnumerator() => Type is not @_.Value ? Element.Elements().Select(_ => new Json(_)).GetEnumerator() : throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}