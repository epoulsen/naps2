/*
    NAPS2 (Not Another PDF Scanner 2)
    http://sourceforge.net/projects/naps2/
    
    Copyright (C) 2009       Pavel Sorejs
    Copyright (C) 2012       Michael Adams
    Copyright (C) 2013       Peter De Leeuw
    Copyright (C) 2012-2014  Ben Olden-Cooligan

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using NAPS2.Scan.Images;

namespace NAPS2.ImportExport.Pdf
{
    public class PrintDocumentPrinter : IImagePrinter
    {
        public void PromptToPrint(List<IScannedImage> images, List<IScannedImage> selectedImages)
        {
            if (!images.Any())
            {
                return;
            }
            var printDialog = new PrintDialog
            {
                AllowSelection = selectedImages.Any(),
                AllowSomePages = true,
                PrinterSettings =
                {
                    MinimumPage = 1,
                    MaximumPage = images.Count,
                    FromPage = 1,
                    ToPage = images.Count
                }
            };
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                Print(printDialog.PrinterSettings, images, selectedImages);
            }
        }

        public void Print(PrinterSettings printerSettings, List<IScannedImage> images, List<IScannedImage> selectedImages)
        {
            List<IScannedImage> imagesToPrint;
            switch (printerSettings.PrintRange)
            {
                case PrintRange.AllPages:
                    imagesToPrint = images;
                    break;
                case PrintRange.Selection:
                    imagesToPrint = selectedImages;
                    break;
                case PrintRange.SomePages:
                    int start = printerSettings.FromPage - 1;
                    int length = printerSettings.ToPage - start;
                    imagesToPrint = images.Skip(start).Take(length).ToList();
                    break;
                default:
                    imagesToPrint = new List<IScannedImage>();
                    break;
            }

            var printDocument = new PrintDocument();
            int i = 0;
            printDocument.PrintPage += (sender, e) =>
            {
                using (var image = imagesToPrint[i].GetImage())
                {
                    var pb = e.PageBounds;
                    var rect = image.Width / pb.Width < image.Height / pb.Height
                            ? new Rectangle(pb.Left, pb.Top, image.Width * pb.Height / image.Height, pb.Height)
                            : new Rectangle(pb.Left, pb.Top, pb.Width, image.Height * pb.Width / image.Width);
                    e.Graphics.DrawImage(image, rect);
                }
                e.HasMorePages = (++i < imagesToPrint.Count);
            };
            printDocument.PrinterSettings = printerSettings;
            printDocument.Print();
        }
    }
}
