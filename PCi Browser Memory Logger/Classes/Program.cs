﻿//  Copyright © 2011-2012 LEGRAND David <david@pcinpact.com>    
//  
//  This file is part of PC INpact Browser Memory Logger.
//
//  PC INpact Browser Memory Logger is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  PC INpact Browser Memory Logger is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with PC INpact Browser Memory Logger.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;

namespace NXi_Browser_Memory_Logger
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
