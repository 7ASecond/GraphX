﻿

using GraphX.Controls.Models;
#if WPF
using System.Windows;
using DefaultEventArgs = System.EventArgs;
using System.Windows.Controls;
#elif METRO
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using DefaultEventArgs = System.Object;
#endif
using GraphX.PCL.Common.Exceptions;

namespace GraphX.Controls
{
#if METRO
    [Bindable]
#endif
    public class AttachableVertexLabelControl : VertexLabelControl, IAttachableControl<VertexControl>
    {
        /// <summary>
        /// Gets label attach node
        /// </summary>
        public VertexControl AttachNode { get { return (VertexControl) GetValue(AttachNodeProperty); } private set {SetValue(AttachNodeProperty, value);} }

        public static readonly DependencyProperty AttachNodeProperty = DependencyProperty.Register("AttachNode", typeof(VertexControl), typeof(AttachableVertexLabelControl), 
            new PropertyMetadata(null));


        public AttachableVertexLabelControl()
        {
            DataContext = this;
        }

        /// <summary>
        /// Attach label to VertexControl
        /// </summary>
        /// <param name="node">VertexControl node</param>
        public void Attach(VertexControl node)
        {
#if WPF
            if (AttachNode != null)
                AttachNode.IsVisibleChanged -= AttachNode_IsVisibleChanged;
            AttachNode = node;
            AttachNode.IsVisibleChanged += AttachNode_IsVisibleChanged;
#elif METRO
            AttachNode = node;
#endif
            node.AttachVertexLabel(this);
        }

#if WPF
        void AttachNode_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AttachNode.IsVisible && AttachNode.ShowLabel)
                Show();
            else if (!AttachNode.IsVisible)
            {
                Hide();
            }
        }
#endif

        protected override VertexControl GetVertexControl(DependencyObject parent)
        {
            if(AttachNode == null)
                throw new GX_InvalidDataException("AttachableVertexLabelControl node is not attached!");
            return AttachNode;
        }

        public override void UpdatePosition()
        {
            if (double.IsNaN(DesiredSize.Width) || DesiredSize.Width == 0) return;

            var vc = GetVertexControl(GetParent());
            if (vc == null) return;

            if (LabelPositionMode == VertexLabelPositionMode.Sides)
            {
                var vcPos = vc.GetPosition();
                Point pt;
                switch (LabelPositionSide)
                {
                    case VertexLabelPositionSide.TopRight:
                        pt = new Point(vcPos.X + vc.DesiredSize.Width, vcPos.Y + -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.BottomRight:
                        pt = new Point(vcPos.X + vc.DesiredSize.Width, vcPos.Y + vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.TopLeft:
                        pt = new Point(vcPos.X + -DesiredSize.Width, vcPos.Y + -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.BottomLeft:
                        pt = new Point(vcPos.X + -DesiredSize.Width, vcPos.Y + vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Top:
                        pt = new Point(vcPos.X + vc.DesiredSize.Width * .5 - DesiredSize.Width * .5, vcPos.Y + -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Bottom:
                        pt = new Point(vcPos.X + vc.DesiredSize.Width * .5 - DesiredSize.Width * .5, vcPos.Y + vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Left:
                        pt = new Point(vcPos.X + -DesiredSize.Width, vcPos.Y + vc.DesiredSize.Height * .5f - DesiredSize.Height * .5);
                        break;
                    case VertexLabelPositionSide.Right:
                        pt = new Point(vcPos.X + vc.DesiredSize.Width, vcPos.Y + vc.DesiredSize.Height * .5f - DesiredSize.Height * .5);
                        break;
                    default:
                        throw new GX_InvalidDataException("UpdatePosition() -> Unknown vertex label side!");
                }
                LastKnownRectSize = new Rect(pt, DesiredSize);
            }
            else LastKnownRectSize = new Rect(LabelPosition, DesiredSize);

            Arrange(LastKnownRectSize);
        }
    }
}
