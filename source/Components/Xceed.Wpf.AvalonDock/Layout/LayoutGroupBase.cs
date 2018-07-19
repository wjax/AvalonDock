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
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
  [Serializable]
  public abstract class LayoutGroupBase : LayoutElement
  {
    #region Internal Methods
    /// <summary>
    /// Log4net logger facility for this class.
    /// </summary>
    protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    protected virtual void OnChildrenCollectionChanged()
    {
      Logger.InfoFormat("_");

      if( ChildrenCollectionChanged != null )
        ChildrenCollectionChanged( this, EventArgs.Empty );
    }

    protected void NotifyChildrenTreeChanged( ChildrenTreeChange change )
    {
      Logger.InfoFormat("_");

      OnChildrenTreeChanged( change );
      var parentGroup = Parent as LayoutGroupBase;
      if( parentGroup != null )
        parentGroup.NotifyChildrenTreeChanged( ChildrenTreeChange.TreeChanged );
    }

    protected virtual void OnChildrenTreeChanged( ChildrenTreeChange change )
    {
      Logger.InfoFormat("_");

      if( ChildrenTreeChanged != null )
        ChildrenTreeChanged( this, new ChildrenTreeChangedEventArgs( change ) );
    }

    #endregion

    #region Events

    [field: NonSerialized]
    [field: XmlIgnore]
    public event EventHandler ChildrenCollectionChanged;

    [field: NonSerialized]
    [field: XmlIgnore]
    public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;

    #endregion
  }
}
