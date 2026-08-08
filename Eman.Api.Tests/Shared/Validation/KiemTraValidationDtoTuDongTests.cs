using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;

namespace Eman.Api.Tests.Shared;

/// <summary>
/// Tự kiểm tra các DataAnnotations của DTO hiện tại và DTO mới trong Application.
/// Khi thêm [Required], [MaxLength] hoặc [Range], test này tự nhận diện.
/// </summary>
public sealed class KiemTraValidationDtoTuDongTests
{
    [Fact(DisplayName = "Mọi Required, MaxLength và Range trong DTO phải thực sự phát sinh lỗi validation")]
    public void TatCaDataAnnotation_PhaiHoatDong()
    {
        var assembly = typeof(TaoVatTuRequest).Assembly;
        var loi = new List<string>();
        var soQuyTacDaKiemTra = 0;

        var dtoTypes = assembly.GetTypes()
            .Where(type => type.IsClass
                && !type.IsAbstract
                && type.Namespace?.Contains(".Dtos", StringComparison.Ordinal) == true
                && type.GetConstructor(Type.EmptyTypes) is not null)
            .OrderBy(type => type.FullName)
            .ToArray();

        foreach (var dtoType in dtoTypes)
        {
            foreach (var property in dtoType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                foreach (var attribute in property.GetCustomAttributes<ValidationAttribute>())
                {
                    object? giaTriKhongHopLe = attribute switch
                    {
                        RequiredAttribute when property.PropertyType == typeof(string) => string.Empty,
                        RequiredAttribute => null,
                        MaxLengthAttribute maxLength when property.PropertyType == typeof(string)
                            => new string('A', maxLength.Length + 1),
                        RangeAttribute range => TaoGiaTriNgoaiKhoang(property.PropertyType, range),
                        _ => KhongHoTro.Value
                    };

                    if (ReferenceEquals(giaTriKhongHopLe, KhongHoTro.Value))
                    {
                        continue;
                    }

                    soQuyTacDaKiemTra++;
                    var instance = Activator.CreateInstance(dtoType)!;

                    try
                    {
                        property.SetValue(instance, giaTriKhongHopLe);
                    }
                    catch (Exception exception)
                    {
                        loi.Add($"Không gán được dữ liệu kiểm thử cho {dtoType.Name}.{property.Name}: {exception.Message}");
                        continue;
                    }

                    var results = new List<ValidationResult>();
                    Validator.TryValidateObject(
                        instance,
                        new ValidationContext(instance),
                        results,
                        validateAllProperties: true);

                    var propertyCoLoi = results.Any(result =>
                        result.MemberNames.Contains(property.Name, StringComparer.Ordinal));

                    if (!propertyCoLoi)
                    {
                        loi.Add(
                            $"{dtoType.Name}.{property.Name} có {attribute.GetType().Name} " +
                            "nhưng dữ liệu sai không phát sinh lỗi validation.");
                    }
                }
            }
        }

        Assert.True(soQuyTacDaKiemTra > 0, "Không tìm thấy quy tắc DataAnnotations nào để kiểm tra.");
        Assert.True(loi.Count == 0,
            $"Phát hiện DataAnnotations không hoạt động:{Environment.NewLine}{string.Join(Environment.NewLine, loi)}");
    }

    private static object? TaoGiaTriNgoaiKhoang(Type propertyType, RangeAttribute range)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (type != typeof(byte)
            && type != typeof(short)
            && type != typeof(int)
            && type != typeof(long)
            && type != typeof(float)
            && type != typeof(double)
            && type != typeof(decimal))
        {
            return KhongHoTro.Value;
        }

        if (!decimal.TryParse(
                Convert.ToString(range.Minimum, CultureInfo.InvariantCulture),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var minimum)
            || !decimal.TryParse(
                Convert.ToString(range.Maximum, CultureInfo.InvariantCulture),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var maximum))
        {
            return KhongHoTro.Value;
        }

        var invalid = minimum > LayGiaTriNhoNhat(type)
            ? minimum - 1
            : maximum + 1;

        return Convert.ChangeType(invalid, type, CultureInfo.InvariantCulture);
    }

    private static decimal LayGiaTriNhoNhat(Type type)
    {
        if (type == typeof(byte)) return byte.MinValue;
        if (type == typeof(short)) return short.MinValue;
        if (type == typeof(int)) return int.MinValue;
        if (type == typeof(long)) return long.MinValue;
        if (type == typeof(float)) return decimal.MinValue;
        if (type == typeof(double)) return decimal.MinValue;
        return decimal.MinValue;
    }

    private static class KhongHoTro
    {
        public static readonly object Value = new();
    }
}
