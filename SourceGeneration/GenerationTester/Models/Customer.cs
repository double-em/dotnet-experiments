﻿using SourceGeneration.DataModel;

namespace GenerationTester.Models;

[DataModel]
public partial class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}