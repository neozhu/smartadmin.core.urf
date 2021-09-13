using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartAdmin.WebUI.Extensions
{
  public static class StringExtensions
  {
    public static string ToMD5(this string input)
    {
      using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      {
        var encoding = Encoding.ASCII;
        var data = encoding.GetBytes(input);

        Span<byte> hashBytes = stackalloc byte[16];
        md5.TryComputeHash(data, hashBytes, out int written);
        if (written != hashBytes.Length)
          throw new OverflowException();


        Span<char> stringBuffer = stackalloc char[32];
        for (int i = 0; i < hashBytes.Length; i++)
        {
          hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
        }
        return new string(stringBuffer);
      }
    }
  }
  public static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        public static bool HasItems<T>(this IEnumerable<T> source) => source != null && source.Any();

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || source.Any() == false;

        [DebuggerStepThrough]
        public static List<T> ToSafeList<T>(this IEnumerable<T> source) => new List<T>(source);

        private static readonly JsonSerializerOptions DefaultSettings = SerializerSettings();

        private static JsonSerializerOptions SerializerSettings(bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = indented,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return options;
        }

        [DebuggerStepThrough]
        private static string Serialize<TTarget>(this TTarget source) => JsonSerializer.Serialize(source, DefaultSettings);

        [DebuggerStepThrough]
        private static TTarget Deserialize<TTarget>(this string value) => JsonSerializer.Deserialize<TTarget>(value, DefaultSettings);

        [DebuggerStepThrough]
        public static TTarget MapTo<TTarget>(this object source) => source.MapTo<object, TTarget>();

        [DebuggerStepThrough]
        public static TTarget MapTo<TSource, TTarget>(this TSource source) => source.Serialize().Deserialize<TTarget>();

        [DebuggerStepThrough]
        public static IEnumerable<TTarget> MapTo<TSource, TTarget>(this IEnumerable<TSource> source) => source.Select(element => element.MapTo<TTarget>());
    }
}
