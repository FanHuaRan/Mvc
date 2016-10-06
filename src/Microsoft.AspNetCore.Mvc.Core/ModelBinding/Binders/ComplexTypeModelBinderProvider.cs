// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for complex types.
    /// </summary>
    public class ComplexTypeModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType &&
                !context.Metadata.IsCollectionType &&
                HasDefaultConstructor(context.Metadata.ModelType.GetTypeInfo()))
            {
                var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
                foreach (var property in context.Metadata.Properties)
                {
                    propertyBinders.Add(property, context.CreateBinder(property));
                }

                return new ComplexTypeModelBinder(propertyBinders);
            }

            return null;
        }

        private bool HasDefaultConstructor(TypeInfo modelTypeInfo)
        {
            // The following check would cause the ComplexTypeModelBinder to NOT participate in binding structs.
            // - Even though structs have implicit parameterless constructors they do not show up using reflection.
            // - Also this binder would eventually fail to construct an object out of a struct as the Linq's
            //   NewExpression compile fails to construct it.
            return !modelTypeInfo.IsAbstract && modelTypeInfo.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
