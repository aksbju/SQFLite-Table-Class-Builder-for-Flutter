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
            this.tableCode += $"const String table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} = '{this.tableName.Replace(" ", "_").ToLower()}';";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns)
            {
                this.tableCode += $"const String column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))} = '{each.ColumnName}';";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += Environment.NewLine;
            this.tableCode += $"class {StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} " + "{";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns) 
            {
                if (each.DataType == "INTEGER")
                {
                    this.tableCode += indent + $"late int {each.ColumnName};";
                }
                else if (each.DataType == "TEXT")
                {
                    this.tableCode += indent + $"late String {each.ColumnName};";
                }
                else
                {
                    this.tableCode += indent + $"late dynamic {each.ColumnName};";
                }
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + $"{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}";
            this.tableCode += "({";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns)
            {
                this.tableCode += indent + indent + $"required this.{each.ColumnName},";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + "});";
            this.tableCode += Environment.NewLine;
            ///////////////////////////////////
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
            ///////////////////////////////////
            this.tableCode += indent + $"{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}" + ".fromMap(Map<String, Object?> map) {";
            this.tableCode += Environment.NewLine;
            foreach (var each in columns) 
            {
                this.tableCode += indent + indent + $"{each.ColumnName} = map[column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))}] as {(each.DataType == "INTEGER" ? "int" : each.DataType == "TEXT" ? "String" : "dynamic")};";
                this.tableCode += Environment.NewLine;
            }
            this.tableCode += indent + "}";
            this.tableCode += Environment.NewLine;
            ///////////////////////////////////
            this.tableCode += "}";
        }
        void PrepareTableProviderCode()
        {
            this.tableProviderCode += $"import 'package:sqflite/sqflite.dart';";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += $"import '{this.tableName.Replace(" ", "_").ToLower()}.dart';";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += $"class {StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}Provider " + "{";
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
            this.tableProviderCode += indent + indent + indent + indent + indent + $"create table $table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} (";
            this.tableProviderCode += Environment.NewLine;
            int counter = 0;
            foreach (var each in columns) 
            {
                this.tableProviderCode += indent + indent + indent + indent + indent + indent + $"$column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))} {each.DataType}{(primaryKey == counter ? " PRIMARY KEY" : "")}{(primaryKey == counter && this.autoIncrement ? " AUTOINCREMENT" : "")},";
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


            this.tableProviderCode += Environment.NewLine;


            // insert
            this.tableProviderCode += indent + $"Future insert({StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} {this.tableName.Replace(" ", "_").ToLower()}) async" + " {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + $"await db.insert(table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}, {this.tableName.Replace(" ", "_").ToLower()}.toMap());";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "}";
            this.tableProviderCode += Environment.NewLine;

            this.tableProviderCode += Environment.NewLine;

            // get
            this.tableProviderCode += indent + $"Future<List<{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}>> get({(this.columns[this.primaryKey].DataType == "INTEGER" ? "int" : this.columns[this.primaryKey].DataType == "TEXT" ? "String" : "dynamic") + $" {this.columns[this.primaryKey].ColumnName}"}) async" + " {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + $"List<Map<String, dynamic>> maps = await db.query(table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())},";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + "columns: [";
            this.tableProviderCode += Environment.NewLine;
            foreach (var each in columns)
            {
                this.tableProviderCode += indent + indent + indent + indent + $"column{StringExtensions.ToTitleCase(each.ColumnName.Replace("_", ""))},";
                this.tableProviderCode += Environment.NewLine;
            }


            this.tableProviderCode += indent + indent + indent + "],";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + $"where: '$column{StringExtensions.ToTitleCase(this.columns[this.primaryKey].ColumnName.Replace("_", ""))} = ?',";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + $"whereArgs: [{this.columns[this.primaryKey].ColumnName.Replace("_", "")}]";



            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + ");";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "if (maps.isNotEmpty) {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + indent + $"return " + $"{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}.fromMap(maps.first);";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "}";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "return null;";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "}";
            this.tableProviderCode += Environment.NewLine;

            this.tableProviderCode += Environment.NewLine;

            // delete
            this.tableProviderCode += indent + $"Future delete({StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} {this.tableName.Replace(" ", "_").ToLower()}) async" + " {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "return await db.delete(" +
                Environment.NewLine +
                indent + indent + indent + $"table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}, " +
                Environment.NewLine +
                indent + indent + indent + $"where: '$column{StringExtensions.ToTitleCase(this.columns[this.primaryKey].ColumnName.Replace("_", ""))} = ?'," +
                Environment.NewLine +
                indent + indent + indent + $"whereArgs: [{this.tableName.Replace(" ", "_").ToLower()}.{this.columns[this.primaryKey].ColumnName.Replace("_", "")}]" +
                Environment.NewLine +
                indent + indent + ");";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "}";
            this.tableProviderCode += Environment.NewLine;

            this.tableProviderCode += Environment.NewLine;

            // update
            this.tableProviderCode += indent + $"Future update({StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())} {this.tableName.Replace(" ", "_").ToLower()}) async" + " {";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + indent + "return await db.update(" +
                Environment.NewLine +
                indent + indent + indent + $"table{StringExtensions.ToTitleCase(this.tableName.Replace(" ", "_").ToLower())}, " +
                Environment.NewLine +
                indent + indent + indent + $"{this.tableName.Replace(" ", "_").ToLower()}.toMap()" +
                Environment.NewLine +
                indent + indent + indent + $"where: '$column{StringExtensions.ToTitleCase(this.columns[this.primaryKey].ColumnName.Replace("_", ""))} = ?'," +
                Environment.NewLine +
                indent + indent + indent + $"whereArgs: [{this.tableName.Replace(" ", "_").ToLower()}.{this.columns[this.primaryKey].ColumnName.Replace("_", "")}]" +
                Environment.NewLine +
                indent + indent + ");";
            this.tableProviderCode += Environment.NewLine;
            this.tableProviderCode += indent + "}";
            this.tableProviderCode += Environment.NewLine;

            this.tableProviderCode += Environment.NewLine;

            // close
            this.tableProviderCode += indent + $"Future close() async => db.close();";

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
