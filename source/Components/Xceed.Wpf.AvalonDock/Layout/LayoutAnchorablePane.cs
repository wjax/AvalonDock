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
using System.Linq;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
  [ContentProperty( "Children" )]
  [Serializable]
  public class LayoutAnchorablePane : LayoutPositionableGroup<LayoutAnchorable>, ILayoutAnchorablePane, ILayoutPositionableElement, ILayoutContentSelector, ILayoutPaneSerializable
  {
    #region Members
    /// <summary>
    /// Log4net logger facility for this class.
    /// </summary>
    protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private int _selectedIndex = -1;
    [XmlIgnore]
    private bool _autoFixSelectedContent = true;
    private string _name = null;

    #endregion

    #region Constructors

    public LayoutAnchorablePane()
    {
    }

    public LayoutAnchorablePane( LayoutAnchorable anchorable )
    {
      Children.Add( anchorable );
    }

    #endregion

    #region Properties

    #region CanHide

    public bool CanHide
    {
      get
      {
        return Children.All( a => a.CanHide );
      }
    }

    #endregion

    #region CanClose

    public bool CanClose
    {
      get
      {
        return Children.All( a => a.CanClose );
      }
    }

    #endregion

    #region IsHostedInFloatingWindow

    public bool IsHostedInFloatingWindow
    {
      get
      {
        return this.FindParent<LayoutFloatingWindow>() != null;
      }
    }

    #endregion

    #region Name

    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        if( _name != value )
        {
          _name = value;
          RaisePropertyChanged( "Name" );
        }
      }
    }

    #endregion

    #region SelectedContentIndex

    public int SelectedContentIndex
    {
      get
      {
        return _selectedIndex;
      }
      set
      {
        if( value < 0 ||
            value >= Children.Count )
          value = -1;

        if( _selectedIndex != value )
        {
          RaisePropertyChanging( "SelectedContentIndex" );
          RaisePropertyChanging( "SelectedContent" );
          if( _selectedIndex >= 0 &&
              _selectedIndex < Children.Count )
            Children[ _selectedIndex ].IsSelected = false;

          _selectedIndex = value;

          if( _selectedIndex >= 0 &&
              _selectedIndex < Children.Count )
            Children[ _selectedIndex ].IsSelected = true;

          RaisePropertyChanged( "SelectedContentIndex" );
          RaisePropertyChanged( "SelectedContent" );
        }
      }
    }

    #endregion

    #region SelectedContent

    public LayoutContent SelectedContent
    {
      get
      {
        return _selectedIndex == -1 ? null : Children[ _selectedIndex ];
      }
    }

    #endregion

    #endregion

    #region Overrides

    protected override bool GetVisibility()
    {
      Logger.InfoFormat("_");

      return Children.Count > 0 && Children.Any( c => c.IsVisible );
    }

    protected override void ChildMoved( int oldIndex, int newIndex )
    {
      Logger.InfoFormat("_");

      if( _selectedIndex == oldIndex )
      {
        RaisePropertyChanging( "SelectedContentIndex" );
        _selectedIndex = newIndex;
        RaisePropertyChanged( "SelectedContentIndex" );
      }


      base.ChildMoved( oldIndex, newIndex );
    }

    protected override void OnChildrenCollectionChanged()
    {
      Logger.InfoFormat("_");

      AutoFixSelectedContent();
      for( int i = 0; i < Children.Count; i++ )
      {
        if( Children[ i ].IsSelected )
        {
          SelectedContentIndex = i;
          break;
        }
      }

      RaisePropertyChanged( "CanClose" );
      RaisePropertyChanged( "CanHide" );
      RaisePropertyChanged( "IsDirectlyHostedInFloatingWindow" );
      base.OnChildrenCollectionChanged();
    }

    protected override void OnParentChanged( ILayoutContainer oldValue, ILayoutContainer newValue )
    {
      Logger.InfoFormat("_");

      var oldGroup = oldValue as ILayoutGroup;
      if( oldGroup != null )
        oldGroup.ChildrenCollectionChanged -= new EventHandler( OnParentChildrenCollectionChanged );

      RaisePropertyChanged( "IsDirectlyHostedInFloatingWindow" );

      var newGroup = newValue as ILayoutGroup;
      if( newGroup != null )
        newGroup.ChildrenCollectionChanged += new EventHandler( OnParentChildrenCollectionChanged );

      base.OnParentChanged( oldValue, newValue );
    }

    public override void WriteXml( System.Xml.XmlWriter writer )
    {
      Logger.InfoFormat("_");

      if( _id != null )
        writer.WriteAttributeString( "Id", _id );
      if( _name != null )
        writer.WriteAttributeString( "Name", _name );

      base.WriteXml( writer );
    }

    public override void ReadXml( System.Xml.XmlReader reader )
    {
      Logger.InfoFormat("_");

      if( reader.MoveToAttribute( "Id" ) )
        _id = reader.Value;
      if( reader.MoveToAttribute( "Name" ) )
        _name = reader.Value;

      _autoFixSelectedContent = false;
      base.ReadXml( reader );
      _autoFixSelectedContent = true;
      AutoFixSelectedContent();
    }

#if TRACE
        public override void ConsoleDump(int tab)
        {
          System.Diagnostics.Trace.Write( new string( ' ', tab * 4 ) );
          System.Diagnostics.Trace.WriteLine( "AnchorablePane()" );

          foreach (LayoutElement child in Children)
              child.ConsoleDump(tab + 1);
        }
#endif

    #endregion

    #region Public Methods

    public int IndexOf( LayoutContent content )
    {
      Logger.InfoFormat("_");

      var anchorableChild = content as LayoutAnchorable;
      if( anchorableChild == null )
        return -1;

      return Children.IndexOf( anchorableChild );
    }

    public bool IsDirectlyHostedInFloatingWindow
    {
      get
      {
        var parentFloatingWindow = this.FindParent<LayoutAnchorableFloatingWindow>();
        if( parentFloatingWindow != null )
          return parentFloatingWindow.IsSinglePane;

        return false;
        //return Parent != null && Parent.ChildrenCount == 1 && Parent.Parent is LayoutFloatingWindow;
      }
    }

    #endregion

    #region Internal Methods

    internal void SetNextSelectedIndex()
    {
      Logger.InfoFormat("_");

      SelectedContentIndex = -1;
      for( int i = 0; i < this.Children.Count; ++i )
      {
        if( Children[ i ].IsEnabled )
        {
          SelectedContentIndex = i;
          return;
        }
      }
    }

    internal void UpdateIsDirectlyHostedInFloatingWindow()
    {
      Logger.InfoFormat("_");

      RaisePropertyChanged( "IsDirectlyHostedInFloatingWindow" );
    }

    #endregion

    #region Private Methods

    private void AutoFixSelectedContent()
    {
      if( _autoFixSelectedContent )
      {
        if( SelectedContentIndex >= ChildrenCount )
          SelectedContentIndex = Children.Count - 1;

        if( SelectedContentIndex == -1 && ChildrenCount > 0 )
          SetNextSelectedIndex();
      }
    }

    private void OnParentChildrenCollectionChanged( object sender, EventArgs e )
    {
      RaisePropertyChanged( "IsDirectlyHostedInFloatingWindow" );
    }

    #endregion

    #region ILayoutPaneSerializable Interface

    string _id;

    string ILayoutPaneSerializable.Id
    {
      get
      {
        return _id;
      }
      set
      {
        _id = value;
      }
    }

    #endregion
  }
}
