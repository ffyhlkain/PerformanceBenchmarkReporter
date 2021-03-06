﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityPerformanceBenchmarkReporter.Entities;
using UnityPerformanceBenchmarkReporter.Report;

namespace UnityPerformanceBenchmarkReporter
{
    internal class Program
    {
        private static readonly Dictionary<string, string[]> ExcludedConfigFieldNames = new Dictionary<string, string[]>
        {
            {typeof(EditorVersion).Name, new []{"DateSeconds", "RevisionValue", "Branch"}},
            {typeof(PlayerSettings).Name, new []{"MtRendering", "GraphicsJobs"}}
        };

        private static void Main(string[] args)
        {
            var aggregateTestRunResults = new List<PerformanceTestRunResult>();
            var baselinePerformanceTestRunResults = new List<PerformanceTestRunResult>();
            var baselineTestResults = new List<TestResult>();
            var performanceTestRunResults = new List<PerformanceTestRunResult>();
            var testResults = new List<TestResult>();
            var performanceBenchmark = new PerformanceBenchmark(ExcludedConfigFieldNames);
            var optionsParser = new OptionsParser();

            optionsParser.ParseOptions(performanceBenchmark, args);
            var testResultXmlParser = new TestResultXmlParser();

            if (performanceBenchmark.BaselineResultFilesExist)
            {
                performanceBenchmark.AddBaselinePerformanceTestRunResults(testResultXmlParser, baselinePerformanceTestRunResults, baselineTestResults);

                if (baselinePerformanceTestRunResults.Any())
                {
                    aggregateTestRunResults.AddRange(baselinePerformanceTestRunResults);
                }
                else
                {
                    Environment.Exit(1);
                }
            }

            if (performanceBenchmark.ResultFilesExist)
            {
                performanceBenchmark.AddPerformanceTestRunResults(testResultXmlParser, performanceTestRunResults, testResults, baselineTestResults);

                if (performanceTestRunResults.Any())
                {
                    aggregateTestRunResults.AddRange(performanceTestRunResults);
                }
                else
                {
                    Environment.Exit(1);
                }
            }

            var reportWriter = new ReportWriter(ExcludedConfigFieldNames);
            reportWriter.WriteReport(
                aggregateTestRunResults, 
                performanceBenchmark.MetadataValidator, 
                performanceBenchmark.SigFig, 
                performanceBenchmark.ReportDirPath, 
                performanceBenchmark.BaselineResultFilesExist);
        }
    }
}
