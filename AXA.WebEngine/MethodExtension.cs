// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System.Collections.Generic;

namespace AXA.WebEngine
{
    /// <summary>
    /// The method extension to provide helper functions for List operations
    /// </summary>
    public static class MethodExtensions
    {
        /// <summary>
        /// Add an item to the current List&lt;Variables&gt; element (ContextValues, Global). If the variable of the same name exists, it will be replaced.
        /// </summary>
        /// <param name="this">The working List&lt;Variables&gt; element</param>
        /// <param name="v">The Variable to be added.</param>
        public static void AddItem(this List<Variable> @this, Variable v)
        {
            if (@this.Exists(x => x.Name == v.Name))
            {
                @this.Find(x => x.Name == v.Name).Value = v.Value;
            }
            else
            {
                @this.Add(v);
            }
        }

        /// <summary>
        /// Add an item to the current List&lt;Variables&gt; element (ContextValues, Global). If the variable of the same name exists, it will be replaced.
        /// </summary>
        /// <param name="this">the working List&lt;Variables&gt; element</param>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public static void AddItem(this List<Variable> @this, string name, string value)
        {
            if (@this.Exists(x => x.Name == name))
            {
                @this.Find(x => x.Name == name).Value = value;
            }
            else
            {
                @this.Add(new Variable(name, value));
            }
        }

        /// <summary>
        /// Method extension for ContextValues and Global
        /// </summary>
        /// <param name="this">The working List&lt;Variables&gt;</param>
        /// <param name="name">The name of the parameter.</param>
        public static void RemoveItem(this List<Variable> @this, string name)
        {
            if (@this.Exists(x => x.Name == name))
            {
                var item = @this.Find(x => x.Name == name);
                @this.Remove(item);
            }
        }


        /// <summary>
        /// Get the value of an variable by it's given name from a List&lt;Variables&gt; element.
        /// If the item does not exits, a null value will return;
        /// </summary>
        /// <param name="this"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Item(this List<Variable> @this, string name)
        {
            if (@this.Exists(x => x.Name == name))
            {
                var item = @this.Find(x => x.Name == name);
                return item.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
