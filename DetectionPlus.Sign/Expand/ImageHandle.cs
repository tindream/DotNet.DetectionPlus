using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DetectionPlus.HWindowTool;
using HalconDotNet;

namespace DetectionPlus.Sign
{
    public class ModelConfig
    {
        public HTuple ModelID { get; set; }
        public HObject ModelRegion { get; set; }
        public HTuple ModelRow { get; set; }
        public HTuple ModelColumn { get; set; }
        public ModelConfig() { }
    }

    public class ImageHandle
    {
        public static bool CheckFunction(HWindowTool.HWindowTool hWindow, HObject ho_Image, HObject ho_CheckRegion, ModelConfig modelConfig, double minThreshold, double maxThreshold, double minScore, double minArea)
        {
            bool b = true;

            // Local iconic variables 

            HObject ho_ImageReduced = null;
            HObject ho_ModelContours;
            HObject ho_ContoursAffineTrans, ho_RegionAffineTrans = null;
            HObject ho_Region = null, ho_ConnectedRegions = null, ho_RegionOpening = null;
            HObject ho_RegionUnion = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WindowHandle = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Angle1 = new HTuple(), hv_Score = new HTuple();
            HTuple hv_HomMat2D = new HTuple(), hv_ImageFiles = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_HomMat2D1 = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple();
            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_ImageReduced);


            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            try
            {
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                // 获取模型轮廓
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, modelConfig.ModelID, 1);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_Image, modelConfig.ModelID, (new HTuple(0)).TupleRad()
                        , (new HTuple(360)).TupleRad(), 0.6, 1.5, 0.2, 1, 0.5, "least_squares",
                        0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }
                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(hv_Score.TupleGreater(minScore))) != 0)
                    {
                        hWindow.ViewController.AddMessage("Score:" + hv_Score.ToString(), Colors.Lime, 10, 10, AnchorType.LeftTop, CoordSystemType.window, false);
                        hv_HomMat2D.Dispose();
                        HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_Row, hv_Column, hv_Angle, out hv_HomMat2D);
                        ho_ContoursAffineTrans.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffineTrans,
                            hv_HomMat2D);
                        hv_HomMat2D1.Dispose();
                        HOperatorSet.VectorAngleToRigid(modelConfig.ModelRow, modelConfig.ModelColumn, 0, hv_Row,
                            hv_Column, hv_Angle, out hv_HomMat2D1);
                        ho_RegionAffineTrans.Dispose();
                        HOperatorSet.AffineTransRegion(ho_CheckRegion, out ho_RegionAffineTrans,
                            hv_HomMat2D1, "nearest_neighbor");

                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_RegionAffineTrans, out ho_ImageReduced
                            );
                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, minThreshold, maxThreshold);
                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                        ho_RegionOpening.Dispose();
                        HOperatorSet.OpeningCircle(ho_ConnectedRegions, out ho_RegionOpening, 3.5);
                        ho_RegionUnion.Dispose();
                        HOperatorSet.Union1(ho_RegionOpening, out ho_RegionUnion);
                        hv_Area.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row2, out hv_Column2);

                        if ((int)(new HTuple(hv_Area.TupleLess(minArea))) != 0)
                        {
                            hWindow.ViewController.AddMessage("NG", Colors.Red, 10, 35, AnchorType.LeftTop, CoordSystemType.window, false);
                            //  hWindow.repaint(); //刷新显示
                            hWindow.DisplayHObject(ho_RegionAffineTrans, Colors.Red, DrawModelType.margin);
                            hWindow.DisplayHObject(ho_ContoursAffineTrans, Colors.Red, DrawModelType.margin);
                            b = false;

                        }
                        else
                        {
                            hWindow.ViewController.AddMessage("OK:" + hv_Area.ToString(), Colors.Lime, 10, 35, AnchorType.LeftTop, CoordSystemType.window, false);
                            //hWindow.repaint(); //刷新显示
                            hWindow.DisplayHObject(ho_RegionAffineTrans, Colors.Lime, DrawModelType.margin);
                            hWindow.DisplayHObject(ho_ContoursAffineTrans, Colors.Lime, DrawModelType.margin);


                        }



                    }
                    else
                    {
                        hWindow.ViewController.AddMessage("Low Score:" + hv_Score.ToString(), Colors.Red, 10, 10, AnchorType.LeftTop, CoordSystemType.window, false);
                        // hWindow.repaint(); //刷新显示
                        b = false;
                    }
                }
                else
                {

                    hWindow.ViewController.AddMessage("Not Find Model", Colors.Red, 10, 10, AnchorType.LeftTop, CoordSystemType.window, false);
                    // hWindow.repaint(); //刷新显示
                    b = false;
                }

            }
            catch (HalconException exp)
            {
                return false;
            }
            finally
            {
                hWindow.Repaint(); //刷新显示
                ho_ImageReduced.Dispose();

                ho_ModelContours.Dispose();
                ho_ContoursAffineTrans.Dispose();
                ho_RegionAffineTrans.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionUnion.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WindowHandle.Dispose();

                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Angle1.Dispose();
                hv_Score.Dispose();
                hv_HomMat2D.Dispose();
                hv_ImageFiles.Dispose();
                hv_Index.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Scale.Dispose();
                hv_HomMat2D1.Dispose();
                hv_Area.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
            }
            return b;

        }




        public static void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
        HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Column: The column coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically...
            //   - if |Row| == |Column| == 1: for each new textline
            //   = else for each text position.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow
            //       otherwise -> use given string as color string for the shadow color
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply:
            //- Multiple text positions can be defined by specifying a tuple
            //  with multiple Row and/or Column coordinates, i.e.:
            //  - |Row| == n, |Column| == n
            //  - |Row| == n, |Column| == 1
            //  - |Row| == 1, |Column| == n
            //- If |Row| == |Column| == 1,
            //  each element of String is display in a new textline.
            //- If multiple positions or specified, the number of Strings
            //  must match the number of positions, i.e.:
            //  - Either |String| == n (each string is displayed at the
            //                          corresponding position),
            //  - or     |String| == 1 (The string is displayed n times).
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName.Dispose();
            hv_GenParamName = new HTuple();
            hv_GenParamValue.Dispose();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP = "image";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            hv_Color_COPY_INP_TMP.Dispose();
            hv_Column_COPY_INP_TMP.Dispose();
            hv_CoordSystem_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_GenParamName.Dispose();
            hv_GenParamValue.Dispose();

            return;
        }
    }
}
