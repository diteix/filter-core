using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FilterCore.Attributes;
using FilterCore.Enums;
using FilterCore.Interfaces;

namespace FilterCore
{
    public static class FilterCore
    {
        public static IQueryable<T> Where<T, TFilter>(this IQueryable<T> queryable, TFilter filter) where TFilter : IFilter
        {
            return queryable.Where<T>(BuildPredicate<T, TFilter>(filter));
        }

        public static IEnumerable<T> Where<T, TFilter>(this IEnumerable<T> enumerable, TFilter filter) where TFilter : IFilter
        {
            var predicate = BuildPredicate<T, TFilter>(filter).Compile();
            return enumerable.Where<T>(predicate);
        }

        public static IQueryable<T> WherePaged<T, TFilter, TKey>(this IQueryable<T> queryable, TFilter filter, Expression<Func<T, TKey>> orderBy) where TFilter : IFilter
        {
            return queryable.Where<T>(BuildPredicate<T, TFilter>(filter))
            .OrderBy(orderBy)
            .Skip(filter.PageSize * (filter.CurrentPage - 1))
            .Take(filter.PageSize);
        }

        public static IEnumerable<T> WherePaged<T, TFilter, TKey>(this IEnumerable<T> enumerable, TFilter filter, Func<T, TKey> orderBy) where TFilter : IFilter
        {
            return enumerable.Where<T>(BuildPredicate<T, TFilter>(filter).Compile())
            .OrderBy(orderBy)
            .Skip(filter.PageSize * (filter.CurrentPage - 1))
            .Take(filter.PageSize);
        }

        private static Expression<Func<T, bool>> BuildPredicate<T, TFilter>(TFilter filter) where TFilter : IFilter
        {
            ParameterExpression modelObject = Expression.Parameter(typeof(T), "model");

            Expression expression = null;

            foreach (var property in typeof(TFilter).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var propertyValue = property.GetValue(filter);

                foreach (FilterAttribute attribute in property.GetCustomAttributes(typeof(FilterAttribute), true))
                {
                    if (string.IsNullOrEmpty(attribute.FieldToCompare))
                    {
                        continue;
                    }

                    var propertyConstantValue = Expression.Constant(propertyValue, property.PropertyType);

                    if (!attribute.CompareToNull && (propertyValue == null || 
                        (bool)Expression.Lambda(Expression.Equal(propertyConstantValue, Expression.Default(property.PropertyType))).Compile().DynamicInvoke()))
                    {
                        continue;
                    }

                    Expression leftExpression = modelObject;
                    Expression nullCheck = null;

                    var members = attribute.FieldToCompare.Split('.');

                    for(int i = 0; i < members.Length; i++)
                    {
                        leftExpression = Expression.PropertyOrField(leftExpression, members[i]);

                        if (i < members.Length - 1 && nullCheck != null)
                        {
                            nullCheck = BuildConcatExpression(nullCheck, Expression.NotEqual(leftExpression, Expression.Default(leftExpression.Type)));
                        }
                        else if (i < members.Length - 1)
                        {
                            nullCheck = Expression.NotEqual(leftExpression, Expression.Default(leftExpression.Type));
                        }
                    }

                    Expression opperatedExpression = BuildApplyOperator(attribute.OperatorToApply, leftExpression, propertyConstantValue);

                    if (expression != null)
                    {
                        expression = BuildConcatExpression(expression, opperatedExpression);
                    }
                    else
                    {
                        expression = opperatedExpression;
                    }

                    if (nullCheck != null)
                    {
                        expression = BuildConcatExpression(nullCheck, expression);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>((expression == null ? Expression.Constant(true) : expression), modelObject);
        }

        private static Expression BuildApplyOperator(FilterOperator filterOperator, Expression leftExpression, Expression rightExpression)
        {
            switch (filterOperator)
            {
                case FilterOperator.Equal:
                    return Expression.Equal(leftExpression, rightExpression);
                case FilterOperator.NotEqual:
                    return Expression.NotEqual(leftExpression, rightExpression);
                case FilterOperator.LessThan:
                    return Expression.LessThan(leftExpression, rightExpression);
                case FilterOperator.GreaterThan:
                    return Expression.GreaterThan(leftExpression, rightExpression);
                case FilterOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(leftExpression, rightExpression);
                case FilterOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(leftExpression, rightExpression);
                case FilterOperator.Like:
                    return Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) }), rightExpression);
                case FilterOperator.NotLike:
                    return Expression.Not(
                        Expression.Call(
                            leftExpression, 
                            typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) }), 
                            rightExpression));
                case FilterOperator.In:
                    return Expression.Call(rightExpression, typeof(Enumerable).GetMethod(nameof(Enumerable.Contains), new Type[] { leftExpression.Type }), leftExpression);
                case FilterOperator.NotIn:
                    return Expression.Not(Expression.Call(
                        rightExpression, 
                        typeof(Enumerable).GetMethod(nameof(Enumerable.Contains), new Type[] { leftExpression.Type }), 
                        leftExpression));
                case FilterOperator.StartsWith:
                    return Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) }), rightExpression);
                case FilterOperator.EndsWith:
                    return Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) }), rightExpression);
                default:
                    return Expression.Constant(true);
            }
        }

        private static BinaryExpression BuildConcatExpression(Expression leftExpression, Expression rightExpression, BitwiseOperator bitwiseOperator = BitwiseOperator.AND)
        {
            Func<Expression, Expression, BinaryExpression> expression = Expression.AndAlso;

            if (bitwiseOperator == BitwiseOperator.OR) 
            {
                expression = Expression.OrElse;
            }

            return expression(leftExpression, rightExpression);
        }
    }
}
