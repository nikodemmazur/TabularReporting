### What is it?
Extensible framework for creating tabular reports from any type.

![What is it?](/images/what_is_it.png)
### Terminology
- **source** - A type `T` implementing `IEnumerable<T>` from which you want to create a **report**.
- **report** - Composition of **rows** and **columns**.
- **row** - An element of a **report**, contained by **column**. It must contain at least one **column**. It is never an **endpoint** of a **report**.
- **column** - An element of a **report** (report is a **column**, not a **row**). It can contain either **rows** or an **endpoint**.
- **endpoint** - An instance of **object**. It carries a useful information you want to put in a **report**.
- **reporter** - Creates a **report** as `IColumn` from **source** using **queries**.
- **query** - The information about how to process a **source** to get **rows** or a **column**.
- **interpreting** - The act of translating a report as `IColumn` to real-world data fields you extracted from **source**.
- **branching** - Continuing the **reporter** projection in a recursive manner.
### Example
The `TestResult` reporting procedure is as follows:
1. Get instances from which you want to create a **report**.
2. Tell what your **report** should look like using **queries**.
3. Fire **reporter** as `IReporter` which does the projection.
4. Format the obtained xml-like **report** as `IColumn` using your `IFormatter` or the ready one `SimpleTextFormatter` (shipped with this framework).
   Then, the result is `System.string`, `System.IO.TextWriter` or any other class you have chosen to represent your in-memory **report**.
5. Save your **report** to disk.

Optionally:

6. Read your **report** back.
7. Parse your **report** back to `IColumn`.
8. Interpret `IColumn` - translate its **rows** and **columns** to desired data types.
#### Type
Let's introduce an example type that roughly corresponds to the unit test results tree in some IDE.
```csharp
public class TestResult
{
    public string TestName { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public bool Result { get; set; }
    public string AssertType { get; set; }
    public IEnumerable<TestResult> InnerTests { get; set; }

    public TestResult(string testName, TimeSpan executionTime,
        bool result, string assertType, IEnumerable<TestResult> innerTests)
    {
        // ...
    }

    // ...
}
```
#### Report --> Format --> Write
```csharp
using TabularReporting.Abstractions;
using TabularReporting;

// ...

// 1. Prepare result
IEnumerable<TestResult> result = new[] {
        new TestResult("TestMethod0", TimeSpan.FromSeconds(2), true, "Equal", null),
        new TestResult("TestMethod1", TimeSpan.FromSeconds(2), false, "MultiTrue",
            new TestResult("Subtest0", TimeSpan.FromSeconds(1), true, "True", null),
            new TestResult("Subtest1", TimeSpan.FromSeconds(1), false, "True", null)),
        new TestResult("TestMethod2", TimeSpan.FromSeconds(2), true, "Contains", null),
        new TestResult("TestMethod3", TimeSpan.FromSeconds(2), true, "Equal", null),
        new TestResult("TestMethod4", TimeSpan.FromSeconds(2), true, "Empty", null) };

// 2. Define column (report) query
IColumnQuery counterColQuery = new CounterColumnQuery(); // Mutable class - it's an ordinal column
IEnumerable<IRowQuery> reportQueries = new IRowQuery[] {
        new OneTimeRowQuery( // 2a. Define first header row
            new ColumnWithStrQuery("Date"),
            new ColumnWithStrQuery(DateTime.Now.ToShortDateString())),
        new OneTimeRowQuery( // 2b. Define second header row
            new ColumnWithStrQuery("Tested by"),
            new ColumnWithStrQuery("Me")),
        new OneTimeRowQuery( // 2c. Define third header row
            new ColumnWithStrQuery("Final result"),
            new ColumnWithStrQuery(result.All(tr => tr.Result) ? "Passed" : "Failed")),
        // 2c. Define body
        new ByAssertTypeFilter("Equal",
            counterColQuery,
            new NameGetter(),
            new ExecTimeInSecondsGetter(),
            new ResultGetter()),
        new ByAssertTypeFilter("MultiTrue",
            counterColQuery,
            new NameGetter(),
            new ExecTimeInSecondsGetter(),
            new ColumnWithRowsBranchedQuery<TestResult>(tr => tr.InnerTests,
                new EveryRowQuery(
                    new NameGetter(),
                    new ExecTimeInSecondsGetter(),
                    new ResultGetter()))
        ) };

// 3. Report
IColumn reportedColumn =
    new Reporter<TestResult>().Report(result, reportQueries);

// 4. Format
string formattedReport = new SimpleTextFormatter().Format(reportedColumn);

// 5. Write
string reportPath = new SimpleTextWriter().WriteReport(formattedReport, Path.GetTempPath(), "MyReport");
```
#### Read --> Parse --> Interpret
```csharp
// 6. Read
string readReport = new SimpleTextReader().ReadReport(reportPath);
Assert.Equal(formattedReport, readReport);

// 7. Parse
IColumn parsedColumn = new SimpleTextParser().Parse(readReport);

// 8. Interpret
string finalResult = parsedColumn[ColumnLocation.Root.Nest(2, 1)].ToString();
// etc...
```
`reportedColumn` (`IColumn`):
```xml
<Column>
  <Row>
    <Column>Date</Column>
    <Column>21.03.2020</Column>
  </Row>
  <Row>
    <Column>Tested by</Column>
    <Column>Me</Column>
  </Row>
  <Row>
    <Column>Final result</Column>
    <Column>Failed</Column>
  </Row>
  <Row>
    <Column>0</Column>
    <Column>TestMethod0</Column>
    <Column>2</Column>
    <Column>True</Column>
  </Row>
  <Row>
    <Column>1</Column>
    <Column>TestMethod1</Column>
    <Column>2</Column>
    <Column>
      <Row>
        <Column>Subtest0</Column>
        <Column>1</Column>
        <Column>True</Column>
      </Row>
      <Row>
        <Column>Subtest1</Column>
        <Column>1</Column>
        <Column>False</Column>
      </Row>
    </Column>
  </Row>
  <Row>
    <Column>2</Column>
    <Column>TestMethod3</Column>
    <Column>2</Column>
    <Column>True</Column>
  </Row>
</Column>
```
`formattedReport` (`string`):
```none
+--------------------------------------------+
¦ Date                  ¦ 21.03.2020         ¦
¦-----------------------+--------------------¦
¦ Tested by             ¦ Me                 ¦
¦-----------------------+--------------------¦
¦ Final result          ¦ Failed             ¦
¦--------------------------------------------¦
¦ 0 ¦ TestMethod0 ¦ 2 ¦ True                 ¦
¦---+-------------+---+----------------------¦
¦ 1 ¦ TestMethod1 ¦ 2 ¦ Subtest0 ¦ 1 ¦ True  ¦
¦   ¦             ¦   ¦----------+---+-------¦
¦   ¦             ¦   ¦ Subtest1 ¦ 1 ¦ False ¦
¦---+-------------+---+----------------------¦
¦ 2 ¦ TestMethod3 ¦ 2 ¦ True                 ¦
+--------------------------------------------+
```
### More practical example (TestStand)
You can create **reports** from any type as long as you have some **queries** which tell the **reporter** how to process that type.
#### 1. Decorate `PropertyObject`
Adapt the elementary TestStand type to .NET world.
```csharp
public class EnumerablePropertyObject : PropertyObject, IEnumerable<EnumerablePropertyObject>
{
    readonly PropertyObject _propObj;

    public EnumerablePropertyObject(PropertyObject propObj)
    {
        _propObj = propObj ?? throw new ArgumentNullException(nameof(propObj));
    }

	// ...
	// PropertyObject implementation
	// ...

    public IEnumerator<EnumerablePropertyObject> GetEnumerator()
    {
        if (!_propObj.IsArray()) // If PropertyObject is not an array do not iterate.
            yield break;

        int numElements = _propObj.GetNumElements();
        for (int i = 0; i < numElements; i++)
        {
            yield return new EnumerablePropertyObject(_propObj.GetPropertyObjectByOffset(i,
				PropertyOptions.PropOption_NoOptions));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```
#### 2. Define queries
Tell how to process `PropertyObject`.
```csharp
internal class FormattedValueGetter : ISourcedColumnQuery<EnumerablePropertyObject>
{
    // ...

    public FormattedValueGetter(string prefix, string lookupString, string postfix, string printfFormat = "")
    {
        // ...
    }

    // ...

    EnumerablePropertyObject ISourcedColumnQuery<EnumerablePropertyObject>.Source { get => _source;  set => _source = value; }

    Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
        new Union2<IEnumerable<IRowQuery>, object>.Case2(_prefix +
                                                         _source.GetFormattedValue(_lookupString,
                                                                                   PropertyOptions.PropOption_NoOptions,
                                                                                   _printfFormat,
                                                                                   false,
                                                                                   string.Empty) +
                                                         _postfix);
}

internal class NumericDiff : ISourcedColumnQuery<EnumerablePropertyObject>
{
    // ...
	
    public NumericDiff(double initialValue, string lookupString, string format = "F3")
    {
        // ...
    }

    EnumerablePropertyObject ISourcedColumnQuery<EnumerablePropertyObject>.Source { get => _source; set => _source = value; }

    Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content
    {
        get
        {
            double actual = _source.GetValNumber(_lookupString, PropertyOptions.PropOption_NoOptions);
            double diff = actual - _register;
            _register = actual;
            return new Union2<IEnumerable<IRowQuery>, object>.Case2(string.Format($"{{0:{_format}}}", diff));
        }
    }
	
	public NumericDiff Reset(double newInitialValue)
    {
        _register = newInitialValue;
        return this;
    }
}

internal class ByStepTypeFilter : ISourcedRowQuery<EnumerablePropertyObject>
{
    // ...

    public ByStepTypeFilter(string stepType, IEnumerable<IColumnQuery> colQueries)
    {
        // ...
    }
    
	// ...

    IEnumerable<IColumnQuery> IRowQuery.ColumnQueries => _colQueries;

    EnumerablePropertyObject ISourcedRowQuery<EnumerablePropertyObject>.Source { get => _source; set => _source = value; }

    bool IRowQuery.Predicate =>
        _source.GetValString("TS.StepType", PropertyOptions.PropOption_NoOptions) == _stepType;
}
```
#### 3. Use queries to define report
Use `TabularReporting.TestStand.FluentRowBuilder` for convenient in-TestStand use.
```csharp
// ...

// 1. Define column descriptions.
IRowQuery columnNamesRow =
    FluentRowBuilder.CreateRowBuilder().AddColWithStr("No.")
                                       .AddColWithStr("Name")
                                       .AddColWithStr("Result")
                                       .AddColWithStr("Time")
                                       .BuildOneTimeRow();

// 2. Define rows made from NumericLimitTest steps.
IRowQuery numericLimitTestRow =
    FluentRowBuilder.CreateRowBuilder().AddColCounter()
                                       .AddColWithFormattedValue("TS.StepName")
                                       .AddColWithFormattedValue("Value: ", "Numeric", string.Empty)
                                       .AddColNumericDiff(mainSequenceResult.GetValNumber("TS.StartTime", 0x0), "TS.StartTime")
                                       .BuildRowByStepType("NumericLimitTest");

// 3. Define rows made from MultipleNumericLimitTest steps.
IRowQuery multipleNumericLimitTestRow =
    FluentRowBuilder.CreateRowBuilder().AddColCounter()
                                       .AddColWithFormattedValue("TS.StepName")
                                       .AddColWithRowsFromPropertyObject("Measurement",
                                            FluentRowBuilder.CreateRowBuilder().AddColWithFormattedValue("Data", "%.3f").BuildEveryRow())
                                       .AddColNumericDiff(mainSequenceResult.GetValNumber("TS.StartTime", 0x0), "TS.StartTime")
                                       .BuildRowByStepType("NI_MultipleNumericLimitTest");
```
#### 4. Make the report
```csharp
// 4. Report (the projection happens here).
IColumn reportColumn = new Reporter().Report(mainSequenceResult, columnNamesRow, numericLimitTestRow, multipleNumericLimitTestRow);

// 5. Format to textual table.
string reportStr = new SimpleTextFormatter().Format(reportColumn);

// ...
```
`reportColumn` (`IColumn`):
```xml
<Column>
  <Row>
    <Column>No.</Column>
    <Column>Name</Column>
    <Column>Result</Column>
    <Column>Time</Column>
  </Row>
  <Row>
    <Column>0</Column>
    <Column>Numeric Limit Test 0</Column>
    <Column>Value: 2.500</Column>
    <Column>0.064</Column>
  </Row>
  <Row>
    <Column>1</Column>
    <Column>Multiple Numeric Limit Test 0</Column>
    <Column>
      <Row>
        <Column>0.000</Column>
      </Row>
      <Row>
        <Column>7.000</Column>
      </Row>
    </Column>
    <Column>0.006</Column>
  </Row>
  <Row>
    <Column>2</Column>
    <Column>Numeric Limit Test 1</Column>
    <Column>Value: 2.700</Column>
    <Column>0.004</Column>
  </Row>
  <Row>
    <Column>3</Column>
    <Column>Numeric Limit Test 2</Column>
    <Column>Value: 1.000</Column>
    <Column>1.006</Column>
  </Row>
  <Row>
    <Column>4</Column>
    <Column>Multiple Numeric Limit Test 1</Column>
    <Column>
      <Row>
        <Column>7.000</Column>
      </Row>
      <Row>
        <Column>1.000</Column>
      </Row>
    </Column>
    <Column>0.005</Column>
  </Row>
</Column>
```
`reportStr` (`string`):
```none
+------------------------------------------------------------+
¦ No. ¦ Name                          ¦ Result       ¦ Time  ¦
¦-----+-------------------------------+--------------+-------¦
¦ 0   ¦ Numeric Limit Test 0          ¦ Value: 2.500 ¦ 0.064 ¦
¦-----+-------------------------------+--------------+-------¦
¦ 1   ¦ Multiple Numeric Limit Test 0 ¦ 0.000        ¦ 0.006 ¦
¦     ¦                               ¦--------------¦       ¦
¦     ¦                               ¦ 7.000        ¦       ¦
¦-----+-------------------------------+--------------+-------¦
¦ 2   ¦ Numeric Limit Test 1          ¦ Value: 2.700 ¦ 0.004 ¦
¦-----+-------------------------------+--------------+-------¦
¦ 3   ¦ Numeric Limit Test 2          ¦ Value: 1.000 ¦ 1.006 ¦
¦-----+-------------------------------+--------------+-------¦
¦ 4   ¦ Multiple Numeric Limit Test 1 ¦ 7.000        ¦ 0.005 ¦
¦     ¦                               ¦--------------¦       ¦
¦     ¦                               ¦ 1.000        ¦       ¦
+------------------------------------------------------------+
```
### Future development
- Implement `SimpleTextParser`.
- Include call chain in **reporter** exceptions to make them more readable and thus facilitate composing the **report** **query**.
- Consider introducing bidirectional **queries** to support **interpreting**.
### License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)