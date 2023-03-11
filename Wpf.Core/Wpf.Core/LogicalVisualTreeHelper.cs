using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wpf.Core
{
    /// <summary>Defines logical/visual tree walking methods for acquiring parents or children.</summary>
    public static class LogicalVisualTreeHelper
    {
        #region Private Helper Methods
        private static T GetChild<T>(IList<DependencyObject> objects, string name, Func<DependencyObject, string, T> recursiveDigger) where T : DependencyObject
        {
            foreach (DependencyObject child in objects)
            {
                T childAsT = child as T;

                if (IsNameOrTypeMatch(childAsT, name)) return childAsT;

                childAsT = recursiveDigger(child, name);

                if (childAsT != null) return childAsT;
            }

            return null;
        }

        private static HashSet<T> GetChildren<T>(IList<DependencyObject> objects, Func<DependencyObject, HashSet<T>> accumlator) where T : DependencyObject
        {
            HashSet<T> childrenAsT = new HashSet<T>();

            foreach (DependencyObject child in objects)
            {
                T childAsT = child as T;

                if (childAsT != null)
                    childrenAsT.Add(childAsT);

                childrenAsT.AddRange(accumlator(child));
            }

            return childrenAsT;
        }

        private static T GetParent<T>(this DependencyObject dependencyObject, string name, Func<DependencyObject, DependencyObject> parentRetriever) where T : DependencyObject
        {
            DependencyObject parent = parentRetriever(dependencyObject);
            T parentAsT = parent as T;

            if (IsNameOrTypeMatch(parentAsT, name)) return parentAsT;

            while (parent != null)
            {
                parent = parentRetriever(parent);
                parentAsT = parent as T;

                if (IsNameOrTypeMatch(parentAsT, name)) return parentAsT;
            }

            return null;
        }

        private static HashSet<T> GetParents<T>(this DependencyObject dependencyObject, Func<DependencyObject, DependencyObject> parentRetriever, Func<DependencyObject, HashSet<T>> accumlator)
            where T : DependencyObject
        {
            HashSet<T> matches = new HashSet<T>();

            DependencyObject parent = parentRetriever(dependencyObject);
            T parentAsT = parent as T;

            if (parentAsT != null)
                matches.Add(parentAsT);

            if (parent != null) matches.AddRange(accumlator(parent));

            return matches;
        }

        private static bool IsNameOrTypeMatch(this DependencyObject dependencyObject, string name)
        {
            /*
             * if the instance is not null and no name is provided then return true 
             * because the cast previous to this method call returned an object...
             * type matching
             * 
             * if the instance is not null and a name is provided then only return true
             * if the name matches (the type must derive from FrameworkContentElement or 
             * FrameworkElement (where the name property originates for visual types))
             * 
             * if neither or these two conditions are true then return false...the 
             * cast previous to this method call failed or the name does not match
             */
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (dependencyObject.NameMatch(name)) return true;

                return false;
            }

            if (dependencyObject != null) return true;

            return false;
        }

        private static bool NameMatch(this DependencyObject dependencyObject, string name)
        {
            FrameworkElement fe = dependencyObject as FrameworkElement;

            if (fe != null && fe.Name == name) return true;

            FrameworkContentElement fce = dependencyObject as FrameworkContentElement;

            if (fce != null && fce.Name == name) return true;

            return false;
        }

        #endregion

        #region Ancestor

        /// <summary>Gets an ancestor of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The ancestor of the specified type or null if not found.</returns>
        public static T GetAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetAncestor<T>(null);
        }

        /// <summary>Gets an ancestor of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The ancestor of the specified type or null if not found.</returns>
        public static T GetAncestor<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            DependencyObject logicalParent = LogicalTreeHelper.GetParent(dependencyObject);
            T logicalParentAsT = logicalParent as T;

            if (IsNameOrTypeMatch(logicalParentAsT, name)) return logicalParentAsT;

            DependencyObject visualParent = (dependencyObject is Visual || dependencyObject is Visual3D)
                ? VisualTreeHelper.GetParent(dependencyObject)
                : null;

            T visualParentAsT = visualParent as T;

            if (IsNameOrTypeMatch(visualParentAsT, name)) return visualParentAsT;

            if (logicalParent != null)
            {
                logicalParentAsT = logicalParent.GetAncestor<T>(name);

                if (logicalParentAsT != null) return logicalParentAsT;
            }

            if (visualParent != null)
            {
                visualParentAsT = visualParent.GetAncestor<T>(name);

                return visualParentAsT;
            }

            return null;
        }

        /// <summary>Gets ancestors of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The ancestors of the specified type or an empty collection if no matches found.</returns>
        public static HashSet<T> GetAncestors<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            HashSet<T> parents = new HashSet<T>();

            DependencyObject logicalParent = LogicalTreeHelper.GetParent(dependencyObject);
            T logicalParentAsT = logicalParent as T;

            if (logicalParentAsT != null) parents.Add(logicalParentAsT);

            DependencyObject visualParent = (dependencyObject is Visual || dependencyObject is Visual3D)
                ? VisualTreeHelper.GetParent(dependencyObject)
                : null;

            T visualParentAsT = visualParent as T;

            if (visualParentAsT != null) parents.Add(visualParentAsT);

            if (logicalParent != null)
                parents.AddRange(logicalParent.GetAncestors<T>());

            if (visualParent != null)
                parents.AddRange(visualParent.GetAncestors<T>());

            return parents;
        }

        #endregion

        #region Descendant

        /// <summary>Gets a descendant of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The descendant of the specified type or null if not found.</returns>
        public static T GetDescendant<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetDescendant<T>(null);
        }

        /// <summary>Gets a descendant of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The descendant of the specified type or null if not found.</returns>
        public static T GetDescendant<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            IList<DependencyObject> children = LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>().ToList();

            // in order for the visual tree helper to be of assistance the type must be a Visual or Visual3D
            if (dependencyObject is Visual || dependencyObject is Visual3D)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
                    children.Add(VisualTreeHelper.GetChild(dependencyObject, i));
            }

            return GetChild(children, name, GetDescendant<T>);
        }

        /// <summary>Gets a descendants of the specified type.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>A collection of children of the specified type or an empty collection if not matches found.</returns>
        public static HashSet<T> GetDescendants<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            IList<DependencyObject> children = LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>().ToList();

            // in order for the visual tree helper to be of assistance the type must be a Visual or Visual3D
            if (dependencyObject is Visual || dependencyObject is Visual3D)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
                    children.Add(VisualTreeHelper.GetChild(dependencyObject, i));
            }

            return GetChildren(children, GetDescendants<T>);
        }

        #endregion


        #region Logical

        /// <summary>Gets the logical child.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The child of the specified type or null if not found.</returns>
        public static T GetLogicalChild<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetLogicalChild<T>(null);
        }

        /// <summary>Gets the logical child.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The child of the specified type or null if not found.</returns>
        public static T GetLogicalChild<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            IList<DependencyObject> children =
                LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>().ToList();

            return GetChild(children, name, GetLogicalChild<T>);
        }

        /// <summary>Gets the logical children.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>A collection of children of the specified type or an empty collection if not matches found.</returns>
        public static HashSet<T> GetLogicalChildren<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            IList<DependencyObject> children =
                LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>().ToList();

            return GetChildren(children, GetLogicalChildren<T>);
        }

        /// <summary>Gets the logical parent.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The parent of the specified type or null if not found.</returns>
        public static T GetLogicalParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetLogicalParent<T>(null);
        }

        /// <summary>Gets the logical parent.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The parent of the specified type or null if not found.</returns>
        public static T GetLogicalParent<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            return dependencyObject.GetParent<T>(name, LogicalTreeHelper.GetParent);
        }

        /// <summary>Gets the logical parents.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>A collection of parents of the specified type or an empty collection if no matches found.</returns>
        public static HashSet<T> GetLogicalParents<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetParents(LogicalTreeHelper.GetParent, GetLogicalParents<T>);
        }

        #endregion

        #region Visual

        /// <summary>Gets the visual child.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The child of the specified type or null if not found.</returns>
        public static T GetVisualChild<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetVisualChild<T>(null);
        }

        /// <summary>Gets the visual child.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The child of the specified type or null if not found.</returns>
        public static T GetVisualChild<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            IList<DependencyObject> children = new List<DependencyObject>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
                children.Add(VisualTreeHelper.GetChild(dependencyObject, i));

            return GetChild(children, name, GetVisualChild<T>);
        }

        /// <summary>Gets the visual children.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>A collection of children of the specified type or an empty collection if not matches found.</returns>
        public static HashSet<T> GetVisualChildren<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            IList<DependencyObject> children = new List<DependencyObject>();

            int count = VisualTreeHelper.GetChildrenCount(dependencyObject);

            for (int i = 0; i < count; i++)
                children.Add(VisualTreeHelper.GetChild(dependencyObject, i));

            return GetChildren(children, GetVisualChildren<T>);
        }

        /// <summary>Gets the visual parent.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The parent of the specified type or null if not found.</returns>
        public static T GetVisualParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetVisualParent<T>(null);
        }

        /// <summary>Gets the visual parent.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name to search for. Can be null. If null, type matching is used instead.</param>
        /// <returns>The parent of the specified type or null if not found.</returns>
        public static T GetVisualParent<T>(this DependencyObject dependencyObject, string name) where T : DependencyObject
        {
            return dependencyObject.GetParent<T>(name, VisualTreeHelper.GetParent);
        }

        /// <summary>Gets the visual parents.</summary>
        /// <typeparam name="T">The type of visual to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>A collection of parents of the specified type or an empty collection if no matches found.</returns>
        public static HashSet<T> GetVisualParents<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetParents(VisualTreeHelper.GetParent, GetVisualParents<T>);
        }

        #endregion
    }
}
