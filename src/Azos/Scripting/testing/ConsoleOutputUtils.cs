﻿using System;
using System.Collections.Generic;
using System.Text;

using Azos.IO;
using Azos.Serialization.JSON;
using Azos.Platform;

namespace Azos.Scripting
{
  /// <summary>
  /// Provides extension/utilities for dumping stuff into IConsoleOut which represents a local or remote console
  /// </summary>
  public static class ConsoleOutputUtils
  {

    private static AsyncFlowMutableLocal<IConsolePort> ats_Port = new AsyncFlowMutableLocal<IConsolePort>();

    /// <summary>
    /// Gets ConsoleOut ambient reference, this is a framework-internal method typically controlled by
    /// script runners
    /// </summary>
    public static IConsolePort Port => ats_Port.Value ?? LocalConsolePort.Default;

    /// <summary>
    /// Sets Out ambient reference, this is a framework-internal method typically controlled by
    /// script runners and other system-level components
    /// </summary>
    public static void ____SetConsolePort(IConsolePort value) => ats_Port.Value = value;


    /// <summary>
    /// Directs object output to a differently-named console, this is used to redirect certain dumps
    /// into a different area/window "a named console" within trace/debug tools
    /// </summary>
    /// <example>
    /// Use like so:
    /// <code>
    ///  payload.In("received").See();
    /// </code>
    /// </example>
    public static (IConsoleOut where, object obj) In(this object obj, string consoleName)
      => (Port.GetOrCreate(consoleName.NonBlank(nameof(consoleName))), obj);

    /// <summary>
    /// Directs formatted output to a differently-named console, this is used to redirect certain dumps
    /// into a different area/window "a named console" within trace/debug tools
    /// </summary>
    /// <example>
    /// Use like so:
    /// <code>
    ///  "Name is: {0}".In("people").See(got.Name);
    /// </code>
    /// </example>
    public static (IConsoleOut where, string text) In(this string txt, string consoleName)
      => (Port.GetOrCreate(consoleName.NonBlank(nameof(consoleName))), txt);

    /// <summary>
    /// Writes object into console in JSON format
    /// </summary>
    public static void See(this object obj, JsonWritingOptions options = null)
    {
      Port.DefaultConsole.See(obj, options);
    }

    /// <summary>
    /// Writes object into console in JSON format
    /// </summary>
    public static void See(this (IConsoleOut console, object obj) see, JsonWritingOptions options = null)
    {
      see.console.See(see.obj, options);
    }

    /// <summary>
    /// Writes object into console in JSON format
    /// </summary>
    public static void See(this IConsoleOut console, object obj, JsonWritingOptions options = null)
    {
      console.NonNull(nameof(console));

      var line = obj is string str && options == null ? str :
                 obj is StringBuilder sb && options == null ? sb.ToString() :
                 obj.ToJson(options ?? JsonWritingOptions.PrettyPrintRowsAsMap);

      console.WriteLine(line);
    }


    /// <summary>
    /// Writes object into console in JSON format wit a header
    /// </summary>
    public static void See(this object obj, string header, JsonWritingOptions options = null)
    {
      Port.DefaultConsole.See(obj, header, options);
    }

    /// <summary>
    /// Writes object into console in JSON format
    /// </summary>
    public static void See(this (IConsoleOut console, object obj) see, string header, JsonWritingOptions options = null)
    {
      see.console.See(see.obj, header, options);
    }

    /// <summary>
    /// Writes object into console in JSON format wit a header
    /// </summary>
    public static void See(this IConsoleOut console, object obj, string header, JsonWritingOptions options = null)
    {
      console.NonNull(nameof(console)).WriteLine();
      console.Write(header);
      console.WriteLine(obj.ToJson(options ?? JsonWritingOptions.PrettyPrintRowsAsMap));
      console.WriteLine();
    }


    /// <summary>
    /// Writes formatted string into console
    /// </summary>
    public static void SeeArgs(this string text, params object[] args)
    {
      Port.DefaultConsole.SeeArgs(text, args);
    }

    /// <summary>
    /// Writes object into console in JSON format
    /// </summary>
    public static void SeeArgs(this (IConsoleOut console, string text) see, params object[] args)
    {
      see.console.SeeArgs(see.text, args);
    }

    /// <summary>
    /// Writes formatted string into console
    /// </summary>
    public static void SeeArgs(this IConsoleOut console, string text, params object[] args)
    {
      console.NonNull(nameof(console));

      if (text.IsNullOrWhiteSpace())
      {
        if (args==null || args.Length<1) return;

        for(var i=0; i<args.Length; i++)
          text += $"{{{i}}}  ";
      }

      console.WriteLine(text.Args(args));
    }


    /// <summary> Writes a formatted message with info header </summary>
    public static void Info(this string tpl, params object[] args) => ConsoleUtils.Info(Port.DefaultConsole, tpl.Args(args));
    /// <summary> Writes a formatted message with info header </summary>
    public static void Info(this string tpl, int ln, params object[] args) => ConsoleUtils.Info(Port.DefaultConsole, tpl.Args(args), ln);

    /// <summary> Writes a formatted message with warning header </summary>
    public static void Warning(this string tpl, params object[] args) => ConsoleUtils.Warning(Port.DefaultConsole, tpl.Args(args));
    /// <summary> Writes a formatted message with warning header </summary>
    public static void Warning(this string tpl, int ln, params object[] args) => ConsoleUtils.Warning(Port.DefaultConsole, tpl.Args(args), ln);

    /// <summary> Writes a formatted message with error header </summary>
    public static void Error(this string tpl, params object[] args) => ConsoleUtils.Error(Port.DefaultConsole, tpl.Args(args));
    /// <summary> Writes a formatted message with error header </summary>
    public static void Error(this string tpl, int ln, params object[] args) => ConsoleUtils.Error(Port.DefaultConsole, tpl.Args(args), ln);

  }
}