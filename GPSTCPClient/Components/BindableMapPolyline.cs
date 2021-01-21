using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GPSTCPClient.Components
{
    public class BindableMapPolyline : MapShapeBase
    {
        public BindableMapPolyline()
          : base((Shape)new Polyline())
        {
        }

        protected override PointCollection ProjectedPoints
        {
            get
            {
                return ((Polyline)this.EncapsulatedShape).Points;
            }
            set
            {
                ((Polyline)this.EncapsulatedShape).Points = value;
            }
        }

        public FillRule FillRule
        {
            get
            {
                return (FillRule)this.EncapsulatedShape.GetValue(Polyline.FillRuleProperty);
            }
            set
            {
                this.EncapsulatedShape.SetValue(Polyline.FillRuleProperty, (object)value);
            }
        }
    }
}
