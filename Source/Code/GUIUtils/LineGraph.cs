using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RangedDPS.GUIUtils
{
    /// <summary>
    /// A GUI class that renders a line graph of one or more float functions
    /// 
    /// The graph auto-resizes based on the domain and range of the functions
    /// </summary>
    internal class LineGraph
    {
        // TODO better colors
        private static readonly List<Color> colors = new List<Color>
        {
            new Color(1f, 0f, 0f),
            new Color(0f, 1f, 0f),
            new Color(0f, 0f, 1f),
            new Color(1f, 1f, 0f),
            new Color(0f, 1f, 1f),
            new Color(1f, 0f, 1f)
        };

        private const int DATAPOINTS = 500; // The resolution of the graph

        private readonly List<GraphFunction> functions = new List<GraphFunction>();
        private readonly SimpleCurveDrawerStyle curveDrawerStyle;

        private FloatRange graphDomain = new FloatRange(0f, 40f);
        private Vector2 graphRange = new Vector2(0f, 20f);

        private readonly List<SimpleCurveDrawInfo> curves = new List<SimpleCurveDrawInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RimProfiler.GUIGraph`1"/> class.
        /// </summary>
        public LineGraph()
        {
            curveDrawerStyle = new SimpleCurveDrawerStyle
            {
                DrawMeasures = true,
                DrawPoints = false,
                DrawBackground = true,
                DrawBackgroundLines = false,
                DrawLegend = true,
                DrawCurveMousePoint = true,
                OnlyPositiveValues = true,
                UseFixedSection = true,
                UseFixedScale = true,
                UseAntiAliasedLines = true,
                PointsRemoveOptimization = true,
                MeasureLabelsXCount = 10,
                MeasureLabelsYCount = 5,
                XIntegersOnly = true,
                LabelX = "Range",//TODO .Translate()
            };
        }

        /// <summary>
        /// Adds a new function to the graph
        /// </summary>
        /// <param name="func">Func.</param>
        public void AddFunction(GraphFunction func)
        {
            if (!functions.Contains(func))
            {
                functions.Add(func);
                RecalculateCurves();
            }
        }

        /// <summary>
        /// Adds several function to the graph
        /// </summary>
        /// <param name="funcs">Funcs.</param>
        public void AddFunctions(params GraphFunction[] funcs)
        {
            functions.AddRange(funcs.Where(f => !functions.Contains(f)));
            RecalculateCurves();
        }

        /// <summary>
        /// Removes a function from the graph
        /// </summary>
        /// <param name="func">Func.</param>
        public void RemoveFunction(GraphFunction func)
        {
            if (functions.Remove(func))
                RecalculateCurves();
        }


        /// <summary>
        /// Clears all functions from the graph
        /// </summary>
        public void ClearFunctions()
        {
            functions.Clear();
            RecalculateCurves();
        }

        /// <summary>
        /// Render the graph within the given rect.
        /// </summary>
        /// <param name="graphRect">Rect.</param>
        public void Draw(Rect graphRect, Rect legendRect)
        {
            curveDrawerStyle.FixedSection = graphDomain;
            curveDrawerStyle.FixedScale = graphRange;

            SimpleCurveDrawer.DrawCurves(graphRect, curves, curveDrawerStyle, legendRect: legendRect);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        /// <summary>
        /// Recalculates the curves using the current functions.
        /// </summary>
        private void RecalculateCurves()
        {
            curves.Clear();

            if (functions.Count > 0)
            {
                // Calculate the domain first
                float maxDomain = functions.Select(f => f.DomainMax).Max();
                if (Mathf.Approximately(0f, maxDomain))
                    maxDomain += 1f;

                float step = maxDomain / DATAPOINTS;
                graphDomain.max = maxDomain;

                int colorIndex = 0;
                float maxRange = 0f;
                foreach (GraphFunction func in functions)
                {
                    SimpleCurveDrawInfo curveDrawInfo = new SimpleCurveDrawInfo
                    {
                        color = colors[colorIndex++ % colors.Count],
                        label = func.Label,
                        valueFormat = func.ValueFormat,
                        curve = new SimpleCurve()
                    };

                    for (float x = func.DomainMin; x < func.DomainMax; x += step)
                    {
                        float value = func.GetValueFor(x);
                        curveDrawInfo.curve.Add(new CurvePoint(x, value), false);

                        maxRange = Mathf.Max(maxRange, value);
                    }
                    // Ensure the curves zero out outside of their domains
                    curveDrawInfo.curve.Add(func.DomainMin - 1E-05f, 0);
                    curveDrawInfo.curve.Add(func.DomainMax + 1E-05f, 0);

                    curveDrawInfo.curve.SortPoints();
                    curves.Add(curveDrawInfo);
                }

                graphRange.y = maxRange * 1.05f; // Add a little extra space at the top of the graph
            }
        }
    }

    /// <summary>
    /// A single graphable function to be rendered on a graph
    /// </summary>
    internal abstract class GraphFunction
    {
        /// <summary>
        /// Gets the name of this function.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Label { get; }

        /// <summary>
        /// Gets the minimum domain value this function is valid for.
        /// </summary>
        /// <value>The minimum domain value.</value>
        public abstract float DomainMin { get; }

        /// <summary>
        /// Gets the maximum domain value this functionis valid for.
        /// </summary>
        /// <value>The maximum domain value.</value>
        public abstract float DomainMax { get; }

        /// <summary>
        /// Gets a fomat string used to render the value.
        /// </summary>
        /// <value>The value format.</value>
        public virtual string ValueFormat => "{0}";

        /// <summary>
        /// Gets the data value for the given X value
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="x">The x coordinate.</param>
        public abstract float GetValueFor(float x);
    }
}