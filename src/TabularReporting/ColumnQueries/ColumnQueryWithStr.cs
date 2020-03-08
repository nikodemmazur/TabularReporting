﻿using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;

namespace TabularReporting
{
    public class ColumnQueryWithStr : IColumnQuery
    {
        readonly string _str;

        public ColumnQueryWithStr(string str)
        {
            _str = str ?? throw new ArgumentNullException(nameof(str));
        }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content => new Union2<IEnumerable<IRowQuery>, object>.Case2(_str);
    }
}