/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using Xceed.Wpf.AvalonDock.Layout;
using System.Windows;
using Xceed.Wpf.AvalonDock.Commands;
using System.Windows.Input;
using System.Windows.Data;

namespace Xceed.Wpf.AvalonDock.Controls
{
  public class LayoutDocumentItem : LayoutItem
  {
    #region Members
    /// <summary>
    /// Log4net logger facility for this class.
    /// </summary>
    protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private LayoutDocument _document;

    #endregion

    #region Constructors

    internal LayoutDocumentItem()
    {
    }

    #endregion

    #region Properties

    #region Description

    /// <summary>
    /// Description Dependency Property
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register( "Description", typeof( string ), typeof( LayoutDocumentItem ),
                new FrameworkPropertyMetadata( ( string )null, new PropertyChangedCallback( OnDescriptionChanged ) ) );

    /// <summary>
    /// Gets or sets the Description property.  This dependency property 
    /// indicates the description to display for the document item.
    /// </summary>
    public string Description
    {
      get
      {
        return ( string )GetValue( DescriptionProperty );
      }
      set
      {
        SetValue( DescriptionProperty, value );
      }
    }

    /// <summary>
    /// Handles changes to the Description property.
    /// </summary>
    private static void OnDescriptionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ( ( LayoutDocumentItem )d ).OnDescriptionChanged( e );
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the Description property.
    /// </summary>
    protected virtual void OnDescriptionChanged( DependencyPropertyChangedEventArgs e )
    {
      _document.Description = ( string )e.NewValue;
    }

    #endregion

    #endregion

    #region Overrides

    protected override void Close()
    {
      Logger.InfoFormat("_");

      if( ( _document.Root != null ) && ( _document.Root.Manager != null ) )
      {
        var dockingManager = _document.Root.Manager;
        dockingManager._ExecuteCloseCommand( _document );
      }
    }

    protected override void OnVisibilityChanged()
    {
      Logger.InfoFormat("_");

      if( (_document != null) && (_document.Root != null) )
      {
        _document.IsVisible = ( this.Visibility == Visibility.Visible );

        if( _document.Parent is LayoutDocumentPane )
        {
          ( ( LayoutDocumentPane )_document.Parent ).ComputeVisibility();
        }
      }

      base.OnVisibilityChanged();
    }







    internal override void Attach( LayoutContent model )
    {
      Logger.InfoFormat("_");

      _document = model as LayoutDocument;
      base.Attach( model );
    }

    internal override void Detach()
    {
      Logger.InfoFormat("_");

      _document = null;
      base.Detach();
    }

    #endregion

    #region Private Methods







#endregion
  }
}
