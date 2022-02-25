using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFLite_Table_Class_Builder_for_Flutter
{
    internal class Translator
    {
        string tableCode = string.Empty;
        string tableProviderCode = string.Empty;
        public string tableName;
        List<Column> columns;
        public int primaryKey;
        public bool autoIncrement;
        string indent = "  "; // 4spaces
        public Translator(string tableName, List<Column> columns, int primaryKey, bool autoIncrement)
        {
            this.tableName = tableName;
            this.columns = columns;
            this.primaryKey = primaryKey;
            this.autoIncrement = autoIncrement;
        }

        void PrepareTableCode()
        {
            this.tableCode += $"const String table{StringExtensions.ToTitleCase(tableName)} = '{tableName}';";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns)
            {
                this.tableCode += $"const String column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))} = '{each.ColumnName}';";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += Environment.NewLine;
            this.tableCode += $"class {StringExtensions.ToTitleCase(tableName)} " + "{";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns) 
            {
                if (each.DataType == "INTEGER") 
                {
                    this.tableCode += indent + $"int {each.ColumnName};";
                }
                else if (each.DataType == "TEXT")
                {
                    this.tableCode += indent + $"String {each.ColumnName};";
                }
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + $"{StringExtensions.ToTitleCase(tableName)}";
            this.tableCode += "({";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns)
            {
                this.tableCode += indent + indent + $"required this.{each.ColumnName},";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + "});";
            this.tableCode += Environment.NewLine;
            this.tableCode += indent + "Map<String, Object?> toMap() {";
            this.tableCode += Environment.NewLine;
            this.tableCode += indent + indent + "return <String, Object?>{";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns) 
            {
                this.tableCode += indent + indent + indent + $"column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))}: {each.ColumnName},";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + indent + "};";
            this.tableCode += Environment.NewLine;
            this.tableCode += indent + "}";
            this.tableCode += Environment.NewLine;

            this.tableCode += "}";
        }
        void PrepareTableProviderCode()
        {
            this.tableProviderCode += $"import 'package:sqflite/sqflite.dart';";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += $"import '{this.tableName.Replace(" ", "_").ToLower()}.dart';";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += $"class {StringExtensions.ToTitleCase(tableName)}Provider " + "{";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "late Database db;";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "Future open(String dbPath) async {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "db = await openDatabase(";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + "dbPath, version: 2,";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + "onCreate:";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + indent + "(Database db, int version) async {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + indent + indent + "await db.execute('''";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + indent + indent + $"create table $table{StringExtensions.ToTitleCase(tableName)} (";
            this.tableProviderCode += Environment.NewLine;
            int counter = 0;
            foreach (var each in columns) 
            {
                this.tableProviderCode += indent + indent + indent + indent + indent + indent + $"$column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))} {each.DataType}{(primaryKey == counter ? " PRIMARY KEY" : "")}{(this.autoIncrement ? " AUTOINCREMENT" : "")},";
                this.tableProviderCode += Environment.NewLine;
                counter++;
            }
            this.tableProviderCode += indent + indent + indent + indent + indent + $");";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + indent + indent + $"''');";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + indent + "}";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + ");";
            this.tableProviderCode += Environment.NewLine;

            this.tableProviderCode += indent + "}";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += "}";
        }
        public CodeModel Translate()
        {
            PrepareTableCode();
            PrepareTableProviderCode();
            return new CodeModel { TableCode= this.tableCode, TableProviderCode= this.tableProviderCode};
        }
    }
}
