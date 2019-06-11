/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
  public class LayoutPanelControl : LayoutGridControl<ILayoutPanelElement>, ILayoutControl
  {
    #region Members

    private LayoutPanel _model;

    #endregion

    #region Constructors

    internal LayoutPanelControl( LayoutPanel model )
        : base( model, model.Orientation )
    {
      _model = model;
      this.SizeChanged += LayoutPanelControl_SizeChanged;
    }

     #endregion

    #region Overrides

    protected override void OnFixChildrenDockLengths()
    {
      if( ActualWidth == 0.0 ||
          ActualHeight == 0.0 )
        return;

      var modelAsPositionableElement = _model as ILayoutPositionableElementWithActualSize;
      #region Setup DockWidth/Height for children
      if( _model.Orientation == Orientation.Horizontal )
      {
        if( _model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() )
        {
          for( int i = 0; i < _model.Children.Count; i++ )
          {
            var childContainerModel = _model.Children[ i ] as ILayoutContainer;
            var childPositionableModel = _model.Children[ i ] as ILayoutPositionableElement;

            if( childContainerModel != null &&
                ( childContainerModel.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() ||
                 childContainerModel.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() ) )
            {
              childPositionableModel.DockWidth = new GridLength( 1.0, GridUnitType.Star );
            }
            else if( childPositionableModel != null && childPositionableModel.DockWidth.IsStar )
            {
              var childPositionableModelWidthActualSize = childPositionableModel as ILayoutPositionableElementWithActualSize;

              var widthToSet = Math.Max( childPositionableModelWidthActualSize.ActualWidth, childPositionableModel.DockMinWidth );

              widthToSet = Math.Min( widthToSet, ActualWidth / 2.0 );
              widthToSet = Math.Max( widthToSet, childPositionableModel.DockMinWidth );

              childPositionableModel.DockWidth = new GridLength(
                  widthToSet,
                  GridUnitType.Pixel );
            }
          }
        }
        else
        {
          for( int i = 0; i < _model.Children.Count; i++ )
          {
            var childPositionableModel = _model.Children[ i ] as ILayoutPositionableElement;
            if( !childPositionableModel.DockWidth.IsStar )
            {
              childPositionableModel.DockWidth = new GridLength( 1.0, GridUnitType.Star );
            }
          }
        }
      }
      else
      {
        if( _model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() )
        {
          for( int i = 0; i < _model.Children.Count; i++ )
          {
            var childContainerModel = _model.Children[ i ] as ILayoutContainer;
            var childPositionableModel = _model.Children[ i ] as ILayoutPositionableElement;

            if( childContainerModel != null &&
                ( childContainerModel.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() ||
                 childContainerModel.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() ) )
            {
              childPositionableModel.DockHeight = new GridLength( 1.0, GridUnitType.Star );
            }
            else if( childPositionableModel != null && childPositionableModel.DockHeight.IsStar )
            {
              var childPositionableModelWidthActualSize = childPositionableModel as ILayoutPositionableElementWithActualSize;

              var heightToSet = Math.Max( childPositionableModelWidthActualSize.ActualHeight, childPositionableModel.DockMinHeight );
              heightToSet = Math.Min( heightToSet, ActualHeight / 2.0 );
              heightToSet = Math.Max( heightToSet, childPositionableModel.DockMinHeight );

              childPositionableModel.DockHeight = new GridLength( heightToSet, GridUnitType.Pixel );
            }
          }
        }
        else
        {
          for( int i = 0; i < _model.Children.Count; i++ )
          {
            var childPositionableModel = _model.Children[ i ] as ILayoutPositionableElement;
            if( !childPositionableModel.DockHeight.IsStar )
            {
              childPositionableModel.DockHeight = new GridLength( 1.0, GridUnitType.Star );
            }
          }
        }
      }
      #endregion
    }

    private void LayoutPanelControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var modelWithActualSize = _model as ILayoutPositionableElementWithActualSize;
      Console.WriteLine("LayoutPanelControl ActualSize {0},{1} -> {2},{3}",
          ActualWidth, ActualHeight, modelWithActualSize.ActualWidth, modelWithActualSize.ActualHeight);

      modelWithActualSize.ActualWidth = ActualWidth;
      modelWithActualSize.ActualHeight = ActualHeight;
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        Console.WriteLine("LayoutPanelControl ArrangeOverride Size {0},{1}",
                          arrangeSize.Width, arrangeSize.Height);

        return base.ArrangeOverride(arrangeSize);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Console.WriteLine("LayoutPanelControl MeasureOverride Size {0},{1}",
                          constraint.Width, constraint.Height);

        return base.MeasureOverride(constraint);
    }
    #endregion
    }
}
