using AvalonDock.Controls;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace YieldExportReports.Dock
{
    public class ContentPropertyStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectStyle(item, container);
            }
            Console.WriteLine("Name: " + item.GetType().Name);

            if (container is LayoutDocumentItem)
            {
                return CreateStyle<LayoutDocumentItem>(item, container);
            }
            else if (container is LayoutAnchorableItem)
            {
                return CreateStyle<LayoutAnchorableItem>(item, container);
            }
            else
            {
                return base.SelectStyle(item, container);
            }
        }

        private Style CreateStyle<T>(object item, DependencyObject container) where T : LayoutItem
        {
            var style = new Style();
            style.TargetType = typeof(T);

            var props = item.GetType().GetProperties(BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy
                );
            
            foreach (var prop in props)
            {
                var objAttr = prop.GetCustomAttributes(typeof(ContentPropertyAttribute), true).FirstOrDefault();
                if (objAttr is ContentPropertyAttribute attr)
                {
                    var layoutItem = container as T;
                    var fieldInfo = layoutItem?.GetType().GetField(prop.Name + "Property"
                        , BindingFlags.Public
                        | BindingFlags.Static
                        | BindingFlags.FlattenHierarchy
                        );
                    if (fieldInfo != null)
                    {
                        if (fieldInfo.GetValue(null) is DependencyProperty dp)
                        {
                            //make a new source
                            var binding = new Binding(prop.Name);
                            binding.Source = item;

                            if (attr.BindingMode != BindingMode.Default)
                            {
                                binding.Mode = attr.BindingMode;
                            }

                            //BindingOperations.SetBinding(layoutItem, dp, binding);

                            var setter = new Setter();
                            setter.Property = dp;
                            setter.Value = binding;

                            style.Setters.Add(setter);
                        }
                    }
                }
            }

            return style;
        }
    }
}
