// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// intialize new lists of dependents and dependees
        /// </summary>
        private Dictionary<string, HashSet<string>> Keys = new Dictionary<string, HashSet<string>>();
        private int size;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>


        public DependencyGraph()
        {
            Keys = new Dictionary<string, HashSet<string>>();
            size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return GetSize(); }
        }
        private int GetSize()
        {
            int size = 0;
            foreach (KeyValuePair<String, HashSet<string>> pair in Keys)
            {
                size += pair.Value.Count();
            }
            return size;
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            

            get
            {
                int count = 0;
                foreach(KeyValuePair<String, HashSet<String>> Pair in Keys)
                {
                    if (Pair.Value.Contains(s))
                    {
                        count++;
                    }
                }
                return count;

            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            foreach (KeyValuePair<String,HashSet<String>> Pair in Keys)
            {
                if (Pair.Key == s && Pair.Value != null)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            foreach (KeyValuePair<String, HashSet<String>> Pair in Keys)
            {
                if (Pair.Value.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            try
            {
                return new HashSet<string>(Keys[s]);
            }
            catch (KeyNotFoundException)
            {
                return new HashSet<string>();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<string> result=new HashSet<string>();
            foreach(KeyValuePair<string,HashSet<string>> pair in Keys)
            {
                if (pair.Value.Contains(s))
                {
                    result.Add(pair.Key);
                    continue;
                }
            }
            
                return result;
            
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (Keys.ContainsKey(s) && Keys[s].Contains(t))
            {
                return;
            }
            else if (Keys.ContainsKey(s) && !Keys[s].Contains(t))
            {
                Keys[s].Add(t);
                
            }
            else
            {
                HashSet<string> Value = new HashSet<string>();
                Value.Add(t);
                Keys.Add(s, Value);
                
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            foreach (KeyValuePair<string,HashSet<string>> pair in Keys)
            {
                if (pair.Key==s&&pair.Value.Contains(t))
                {
                    pair.Value.Remove(t);
                    
                }
                else
                {
                    continue;
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (!Keys.ContainsKey(s))
            {
                return;
            }
           
            Keys[s].Clear();
            Keys[s].UnionWith(newDependents);
          
            
         
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            foreach(KeyValuePair<string,HashSet<string>> pair in Keys)
            {
                if (pair.Value.Contains(s))
                {
                    pair.Value.Remove(s);
                   
                }
                
            }
            foreach (string newDee in newDependees)
            {
                if (Keys.ContainsKey(newDee)&&!Keys[newDee].Contains(s))
                {
                    Keys[newDee].Add(s);
                    
                }
                else if(!Keys.ContainsKey(newDee))
                {
                    AddDependency(newDee, s);
                    
                }
            }

        }

    }

}