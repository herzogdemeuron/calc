using System;
using Calc.Core.Filtering;
using Calc.Core.Objects;
using Newtonsoft.Json.Linq;

namespace Calc.Core.TestIntegration;

public class MockData
{
    public List<Tree> Trees = new();
    public List<CalcElement> Elements = new();
    public decimal Area = 100m;
    public decimal gwp123 = 123m;
    public decimal Amount = 0.33m;

    public MockData()
    {
        CreateTrees();
        CreateElements();
    }

    private void CreateTrees()
    {
        var tree1 = new Tree
        {
            Name = "Tree1",
            FilterConfig = new GroupCondition
            {
                Operator = "and",
                Conditions = new List<ConditionContainer>
                {
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "Type Name",
                            Method = "contains",
                            Value = "WAND"
                        }
                    },
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "Type Name",
                            Method = "contains",
                            Value = "ROH"
                        }
                    }
                }
            },
            BranchConfig = new List<string> { "Grouping", "SubGrouping" }
        };

        var tree2 = new Tree
        {
            Name = "Tree2",
            FilterConfig = new GroupCondition
            {
                Operator = "and",
                Conditions = new List<ConditionContainer>
                {
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "Type Name",
                            Method = "contains",
                            Value = "DECK"
                        }
                    },
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "Type Name",
                            Method = "contains",
                            Value = "ROH"
                        }
                    }
                }
            },
            BranchConfig = new List<string> { "Grouping", "SubGrouping" }
        };

        this.Trees.Add(tree1);
        this.Trees.Add(tree2);
    }


    private void CreateElements()
    {
        // Mock walls
        var id1 = Guid.NewGuid().ToString();
        var Fields1 = new Dictionary<string, object>
        {
            { "Type Name", "WAND_ROH_Whatever" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup1" }
        };
        var element1 = new CalcElement(id1, Fields1, area: this.Area);
        this.Elements.Add(element1);

        var id2 = Guid.NewGuid().ToString();
        var Fields2 = new Dictionary<string, object>
        {
            { "Type Name", "WAND_AUSB_Whatever" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup1" }
        };
        var element2 = new CalcElement(id2, Fields2, area: this.Area);
        this.Elements.Add(element2);

        var id3 = Guid.NewGuid().ToString();
        var Fields3 = new Dictionary<string, object>
        {
            { "Type Name", "WAND_ROH_Whatever" },
            { "Grouping", "Group2" },
            { "SubGrouping", "SubGroup2" }
        };
        var element3 = new CalcElement(id3, Fields3, area: this.Area);
        this.Elements.Add(element3);

        // Mock floors
        var id4 = Guid.NewGuid().ToString();
        var Fields4 = new Dictionary<string, object>
        {
            { "Type Name", "DECK_ROH_Whatever" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element4 = new CalcElement(id4, Fields4, area: this.Area);
        this.Elements.Add(element4);

        var id5 = Guid.NewGuid().ToString();
        var Fields5 = new Dictionary<string, object>
        {
            { "Type Name", "DECK_AUSB_Whatever" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element5 = new CalcElement(id5, Fields5, area: this.Area);
        this.Elements.Add(element5);

        var id6 = Guid.NewGuid().ToString();
        var Fields6 = new Dictionary<string, object>
        {
            { "Type Name", "DECK_ROH_Whatever" },
            { "Grouping", "GroupB" },
            { "SubGrouping", "SubGroupB" }
        };
        var element6 = new CalcElement(id6, Fields6, area: this.Area);
        this.Elements.Add(element6);
    }

    public void AssignBuildups(Tree tree)
    {
        var material1 = new Material
        {
            Id = 1,
            Name = "Material1",
            KgCO2eA123 = this.gwp123,
            Category = "Category1"
        };

        var component1 = new BuildupComponent
        {
            Amount = this.Amount,
            Material = material1
        };

        var buildup1 = new Buildup
        {
            Id = 9,
            Name = "Flat Concrete Slab",
            Unit = Unit.m2
        };

        var group1 = new MaterialGroup
        {
            Name = "Group1"
        };

        buildup1.Group = group1;
        buildup1.Components = new List<BuildupComponent>
        {
            component1
        };
        tree.SubBranches[0].Buildup = buildup1;
    }
}
