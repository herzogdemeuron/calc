using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using Calc.Core.Filtering;
using Calc.Core.Objects;

namespace Calc.Core.Tests;

[TestClass]
public class ElementFilterTests
{
    [TestMethod]
    public void FilterElements_Should_ReturnFilteredElements()
    {
        // Arrange
        List<CalcElement> elements = GetMockElements();
        string filterJson = @"{
                ""operator"": ""and"",
                ""conditions"": [
                    {
                        ""type"": ""GroupCondition"",
                        ""operator"": ""or"",
                        ""conditions"": [
                            {
                                ""type"": ""SimpleCondition"",
                                ""condition"": {
                                    ""parameter"": ""Foo"",
                                    ""method"": ""contains"",
                                    ""value"": ""a""
                                }
                            },
                            {
                                ""type"": ""SimpleCondition"",
                                ""condition"": {
                                    ""parameter"": ""Foo"",
                                    ""method"": ""equals"",
                                    ""value"": ""b""
                                }
                            }
                        ]
                    },
                    {
                        ""type"": ""SimpleCondition"",
                        ""condition"": {
                            ""parameter"": ""Bar"",
                            ""method"": ""equals"",
                            ""value"": ""c""
                        }
                    }
                ]
            }";



        GroupCondition filter = JsonConvert.DeserializeObject<GroupCondition>(filterJson);

        ElementFilter elementFilter = new ElementFilter();

        // Act
        List<CalcElement> filteredElements = elementFilter.FilterElements(elements, filter);

        // Assert
        foreach (var element in filteredElements)
        {
            Console.WriteLine(element.Id);
        }
        Assert.AreEqual(2, filteredElements.Count);
        // Perform additional assertions based on your mock data and expected results
    }

    private List<CalcElement> GetMockElements()
    {
        List<CalcElement> elements = new List<CalcElement>
        {
            new CalcElement
            {
                Id = "1",
                Fields = new Dictionary<string, object>
                {
                    { "Foo", "abc" },
                    { "Bar", "c" },
                    { "Baz", "def" }
                }
            },
            new CalcElement
            {
                Id = "2",
                Fields = new Dictionary<string, object>
                {
                    { "Foo", "bcd" },
                    { "Bar", "c" },
                    { "Baz", "xyz" }
                }
            },
            new CalcElement
            {
                Id = "3",
                Fields = new Dictionary<string, object>
                {
                    { "Foo", "b" },
                    { "Bar", "c" },
                    { "Baz", "def" }
                }
            },
            new CalcElement
            {
                Id = "4",
                Fields = new Dictionary<string, object>
                {
                    { "Foo", "pqr" },
                    { "Bar", "c" },
                    { "Baz", "def" }
                }
            },
            new CalcElement
            {
                Id = "5",
                Fields = new Dictionary<string, object>
                {
                    { "Foo", "mno" },
                    { "Bar", "d" },
                    { "Baz", "def" }
                }
            }
        };

        return elements;
    }
}
