# Step 5, IS-A HAS-A Table

|    Class1    |    Class2    |    Relations    |
|:------------:|:------------:|:---------------:|
|    FileCabinetService    |    IRecordValidator    |    Aggregation    |
|    FileCabinetDefaultService    |    FileCabinetService    |    Inheritance    |
|    FileCabinetCustomService    |    FileCabinetService    |    Inheritance    |
|    FileCabinetDefaultService    |    DefaultValidator    |    Aggregation    |
|    FileCabinetCustomService    |    CustomValidator    |    Aggregation    |
|    DefaultValidator    |    IRecordValidator    |    Composition    |
|    CustomValidator    |    IRecordValidator    |    Composition    |



# TODO

-> improve select command.
-> continue debugging commands under create.
...