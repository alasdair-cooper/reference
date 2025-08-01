using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AlasdairCooper.Reference.Api.Data.Types;

public readonly record struct Money(Currency Currency, decimal Amount);