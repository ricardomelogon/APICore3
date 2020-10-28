using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public static class AttributeHelpers
{
    public static Int32 GetMaxLength<T>(Expression<Func<T, string>> propertyExpression)
    {
        return GetPropertyAttributeValue<T, string, MaxLengthAttribute, int>(propertyExpression, attr => attr.Length);
    }

    //Required generic method to get any property attribute from any class
    public static TValue GetPropertyAttributeValue<T, TOut, TAttribute, TValue>(Expression<Func<T, TOut>> propertyExpression, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
    {
        MemberExpression expression = (MemberExpression)propertyExpression.Body;
        PropertyInfo propertyInfo = (PropertyInfo)expression.Member;

        if (!(propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute attr))
        {
            throw new MissingMemberException(typeof(T).Name + "." + propertyInfo.Name, typeof(TAttribute).Name);
        }

        return valueSelector(attr);
    }
}