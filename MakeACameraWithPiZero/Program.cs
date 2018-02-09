﻿using System;
using Raspberry.IO.GeneralPurpose;
using MMALSharp;
using Nito.AsyncEx;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Components;
using System.Threading.Tasks;

namespace MakeACameraWithPiZero
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ConfigForm.Create();
        }        
    }
}
