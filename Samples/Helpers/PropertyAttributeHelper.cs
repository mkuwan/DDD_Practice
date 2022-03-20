using Xunit;


namespace Samples.Helpers
{
    public static class PropertyAttributeHelper
    {
        // 以下のメソッドで type.GetProperty はpublicのみを対象としていますが
        // それ以外も取得する場合は以下のように BindingFlags を使用してください
        //var prop = 
        //    type.GetProperty(name,
        //    BindingFlags.InvokeMethod |
        //    BindingFlags.NonPublic | 
        //    BindingFlags.Instance);


        /// <summary>
        /// 型からプロパティの属性(Attribute)を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T? GetPropertyAttribute<T>(Type type, string name) where T : Attribute
        {
            var property = type.GetProperty(name);
            if (property == null)
            {
                // 指定したプロパティが見つかりませんでした
                return default;
            }

            var attribute = property.GetCustomAttribute<T>();
            if (attribute == null)
            {
                // 属性が記載されていない
                return default;
            }

            return attribute;
        }

        /// <summary>
        /// インスタンスからプロパティの属性(Attribute)を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T? GetPropertyAttribute<T>(object instance, string name) where T : Attribute
        {
            return GetPropertyAttribute<T>(instance.GetType(), name);
        }

        /// <summary>
        /// 型からプロパティの属性を全て取得します
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute>? GetPropertyAttributes(Type type, string name)
        {
            var property = type.GetProperty(name);
            if (property == null)
            {
                return default;
            }

            return property.GetCustomAttributes<Attribute>();
        }

        /// <summary>
        /// インスタンスからプロパティの属性を全て取得します
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute>? GetPropertyAttributes(object instance, string name)
        {
            return GetPropertyAttributes(instance.GetType(), name);
        }
    }


    //-------------------------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// 動作確認テスト
    /// </summary>
    public class PropertyAttributeHelperTest
    {
        [Fact()]
        public void GetPropertyAttributeByTypeTest()
        {
            var testClass = new PropertyTestClass();

            var attribute =
                PropertyAttributeHelper.GetPropertyAttribute<DescriptionAttribute>(testClass.GetType(), nameof(testClass.Name));

            Assert.Equal("test1", attribute.Description);
        }

        [Fact()]
        public void GetPropertyAttributeByInstanceTest()
        {
            var testClass = new PropertyTestClass();

            var attribute = PropertyAttributeHelper.GetPropertyAttribute<DescriptionAttribute>(testClass, nameof(testClass.Name));

            Assert.Equal("test1", attribute.Description);
        }



        [Fact()]
        public void GetPropertyAttributesByTypeTest()
        {
            var testClass = new PropertyTestClass();

            var attributes = PropertyAttributeHelper.GetPropertyAttributes(testClass.GetType(), nameof(testClass.CustomName));

            Assert.NotNull(attributes);
            Assert.Equal(2, attributes?.Count());

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(DescriptionAttribute))
                {
                    Assert.Equal("regular", ((DescriptionAttribute)attribute).Description);
                }

                if (attribute.GetType() == typeof(CustomDescriptionAttribute))
                {
                    Assert.Equal("custom", ((CustomDescriptionAttribute)attribute).CustomDescription);
                }
            }
        }

        [Fact()]
        public void GetPropertyAttributesByInstanceTest()
        {
            var testClass = new PropertyTestClass();

            var attributes = PropertyAttributeHelper.GetPropertyAttributes(testClass, nameof(testClass.CustomName));

            Assert.NotNull(attributes);
            Assert.Equal(2, attributes?.Count());

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(DescriptionAttribute))
                {
                    Assert.Equal("regular", ((DescriptionAttribute)attribute).Description);
                }

                if (attribute.GetType() == typeof(CustomDescriptionAttribute))
                {
                    Assert.Equal("custom", ((CustomDescriptionAttribute)attribute).CustomDescription);
                }
            }

        }

    }


    public class PropertyTestClass
    {
        [Description("test1")]
        public string? Name { get; set; }

        [CustomDescription("custom"), Description("regular")]
        public string? CustomName { get; set; }
    }

    internal class CustomDescriptionAttribute : Attribute
    {
        public string CustomDescription;
        public CustomDescriptionAttribute(string customDescription)
        {
            this.CustomDescription = customDescription;
        }
    }
}
