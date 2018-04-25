﻿using Zidium.Api.Others;
using Xunit;

namespace ApiTests_1._0.Others
{
    
    public class StringHelperTests
    {
        class MyClass
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Test()
        {
            string str = "123456";

            StringHelper.SetMaxLength(ref str, 10);
            Assert.Equal("123456", str);

            StringHelper.SetMaxLength(ref str, 2);
            Assert.Equal("12", str);

            StringHelper.SetMaxLength(ref str, 0);
            Assert.Equal("", str);

            str = null;
            StringHelper.SetMaxLength(ref str, 2);
            Assert.Null(str);

            //var obj = new MyClass()
            //{
            //    Text = "123"
            //};
            //StringHelper.SetMaxLength(ref obj.Text, 3);
        }
    }
}
