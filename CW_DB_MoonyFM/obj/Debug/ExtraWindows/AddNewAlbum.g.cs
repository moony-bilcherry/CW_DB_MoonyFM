#pragma checksum "..\..\..\ExtraWindows\AddNewAlbum.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5BCAC28CB9C26C233B4310C21BB4DF79F0F9A86DA3396E277034250D2C7645C4"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using CW_DB_MoonyFM.ExtraWindows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CW_DB_MoonyFM.ExtraWindows {
    
    
    /// <summary>
    /// AddNewAlbum
    /// </summary>
    public partial class AddNewAlbum : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 118 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox albumName;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox artistNameCombo;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox albumYear;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button attachPhoto;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image coverImage;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addBtn;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button closeBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CW_DB_MoonyFM;component/extrawindows/addnewalbum.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            ((CW_DB_MoonyFM.ExtraWindows.AddNewAlbum)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.albumName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.artistNameCombo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.albumYear = ((System.Windows.Controls.TextBox)(target));
            
            #line 132 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            this.albumYear.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.albumPreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 5:
            this.attachPhoto = ((System.Windows.Controls.Button)(target));
            
            #line 140 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            this.attachPhoto.Click += new System.Windows.RoutedEventHandler(this.attachPhoto_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.coverImage = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.addBtn = ((System.Windows.Controls.Button)(target));
            
            #line 147 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            this.addBtn.Click += new System.Windows.RoutedEventHandler(this.addBtn_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.closeBtn = ((System.Windows.Controls.Button)(target));
            
            #line 153 "..\..\..\ExtraWindows\AddNewAlbum.xaml"
            this.closeBtn.Click += new System.Windows.RoutedEventHandler(this.closeBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

