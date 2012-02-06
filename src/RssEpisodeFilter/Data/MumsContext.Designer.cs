﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace MUMS.RssEpisodeFilter.Data
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class MumsContext : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new MumsContext object using the connection string found in the 'MumsContext' section of the application configuration file.
        /// </summary>
        public MumsContext() : base("name=MumsContext", "MumsContext")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new MumsContext object.
        /// </summary>
        public MumsContext(string connectionString) : base(connectionString, "MumsContext")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new MumsContext object.
        /// </summary>
        public MumsContext(EntityConnection connection) : base(connection, "MumsContext")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<User> User
        {
            get
            {
                if ((_User == null))
                {
                    _User = base.CreateObjectSet<User>("User");
                }
                return _User;
            }
        }
        private ObjectSet<User> _User;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<RssEpisodeItems> RssEpisodeItems
        {
            get
            {
                if ((_RssEpisodeItems == null))
                {
                    _RssEpisodeItems = base.CreateObjectSet<RssEpisodeItems>("RssEpisodeItems");
                }
                return _RssEpisodeItems;
            }
        }
        private ObjectSet<RssEpisodeItems> _RssEpisodeItems;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the User EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToUser(User user)
        {
            base.AddObject("User", user);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the RssEpisodeItems EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToRssEpisodeItems(RssEpisodeItems rssEpisodeItems)
        {
            base.AddObject("RssEpisodeItems", rssEpisodeItems);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="Data", Name="RssEpisodeItems")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class RssEpisodeItems : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new RssEpisodeItems object.
        /// </summary>
        /// <param name="rssEpisodeItemId">Initial value of the RssEpisodeItemId property.</param>
        /// <param name="releaseName">Initial value of the ReleaseName property.</param>
        /// <param name="season">Initial value of the Season property.</param>
        /// <param name="episode">Initial value of the Episode property.</param>
        /// <param name="download">Initial value of the Download property.</param>
        /// <param name="pubDate">Initial value of the PubDate property.</param>
        /// <param name="added">Initial value of the Added property.</param>
        /// <param name="enclosureLength">Initial value of the EnclosureLength property.</param>
        /// <param name="enclosureUrl">Initial value of the EnclosureUrl property.</param>
        public static RssEpisodeItems CreateRssEpisodeItems(global::System.Int32 rssEpisodeItemId, global::System.String releaseName, global::System.Int32 season, global::System.Int32 episode, global::System.Boolean download, global::System.DateTime pubDate, global::System.DateTime added, global::System.Int64 enclosureLength, global::System.String enclosureUrl)
        {
            RssEpisodeItems rssEpisodeItems = new RssEpisodeItems();
            rssEpisodeItems.RssEpisodeItemId = rssEpisodeItemId;
            rssEpisodeItems.ReleaseName = releaseName;
            rssEpisodeItems.Season = season;
            rssEpisodeItems.Episode = episode;
            rssEpisodeItems.Download = download;
            rssEpisodeItems.PubDate = pubDate;
            rssEpisodeItems.Added = added;
            rssEpisodeItems.EnclosureLength = enclosureLength;
            rssEpisodeItems.EnclosureUrl = enclosureUrl;
            return rssEpisodeItems;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 RssEpisodeItemId
        {
            get
            {
                return _RssEpisodeItemId;
            }
            set
            {
                if (_RssEpisodeItemId != value)
                {
                    OnRssEpisodeItemIdChanging(value);
                    ReportPropertyChanging("RssEpisodeItemId");
                    _RssEpisodeItemId = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("RssEpisodeItemId");
                    OnRssEpisodeItemIdChanged();
                }
            }
        }
        private global::System.Int32 _RssEpisodeItemId;
        partial void OnRssEpisodeItemIdChanging(global::System.Int32 value);
        partial void OnRssEpisodeItemIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String ReleaseName
        {
            get
            {
                return _ReleaseName;
            }
            set
            {
                OnReleaseNameChanging(value);
                ReportPropertyChanging("ReleaseName");
                _ReleaseName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("ReleaseName");
                OnReleaseNameChanged();
            }
        }
        private global::System.String _ReleaseName;
        partial void OnReleaseNameChanging(global::System.String value);
        partial void OnReleaseNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Season
        {
            get
            {
                return _Season;
            }
            set
            {
                OnSeasonChanging(value);
                ReportPropertyChanging("Season");
                _Season = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Season");
                OnSeasonChanged();
            }
        }
        private global::System.Int32 _Season;
        partial void OnSeasonChanging(global::System.Int32 value);
        partial void OnSeasonChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Episode
        {
            get
            {
                return _Episode;
            }
            set
            {
                OnEpisodeChanging(value);
                ReportPropertyChanging("Episode");
                _Episode = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Episode");
                OnEpisodeChanged();
            }
        }
        private global::System.Int32 _Episode;
        partial void OnEpisodeChanging(global::System.Int32 value);
        partial void OnEpisodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Boolean Download
        {
            get
            {
                return _Download;
            }
            set
            {
                OnDownloadChanging(value);
                ReportPropertyChanging("Download");
                _Download = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Download");
                OnDownloadChanged();
            }
        }
        private global::System.Boolean _Download;
        partial void OnDownloadChanging(global::System.Boolean value);
        partial void OnDownloadChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime PubDate
        {
            get
            {
                return _PubDate;
            }
            set
            {
                OnPubDateChanging(value);
                ReportPropertyChanging("PubDate");
                _PubDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("PubDate");
                OnPubDateChanged();
            }
        }
        private global::System.DateTime _PubDate;
        partial void OnPubDateChanging(global::System.DateTime value);
        partial void OnPubDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> DuplicateOf
        {
            get
            {
                return _DuplicateOf;
            }
            set
            {
                OnDuplicateOfChanging(value);
                ReportPropertyChanging("DuplicateOf");
                _DuplicateOf = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("DuplicateOf");
                OnDuplicateOfChanged();
            }
        }
        private Nullable<global::System.Int32> _DuplicateOf;
        partial void OnDuplicateOfChanging(Nullable<global::System.Int32> value);
        partial void OnDuplicateOfChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime Added
        {
            get
            {
                return _Added;
            }
            set
            {
                OnAddedChanging(value);
                ReportPropertyChanging("Added");
                _Added = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Added");
                OnAddedChanged();
            }
        }
        private global::System.DateTime _Added;
        partial void OnAddedChanging(global::System.DateTime value);
        partial void OnAddedChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 EnclosureLength
        {
            get
            {
                return _EnclosureLength;
            }
            set
            {
                OnEnclosureLengthChanging(value);
                ReportPropertyChanging("EnclosureLength");
                _EnclosureLength = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("EnclosureLength");
                OnEnclosureLengthChanged();
            }
        }
        private global::System.Int64 _EnclosureLength;
        partial void OnEnclosureLengthChanging(global::System.Int64 value);
        partial void OnEnclosureLengthChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String EnclosureUrl
        {
            get
            {
                return _EnclosureUrl;
            }
            set
            {
                OnEnclosureUrlChanging(value);
                ReportPropertyChanging("EnclosureUrl");
                _EnclosureUrl = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("EnclosureUrl");
                OnEnclosureUrlChanged();
            }
        }
        private global::System.String _EnclosureUrl;
        partial void OnEnclosureUrlChanging(global::System.String value);
        partial void OnEnclosureUrlChanged();

        #endregion
    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="Data", Name="User")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class User : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new User object.
        /// </summary>
        /// <param name="token">Initial value of the Token property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        public static User CreateUser(global::System.String token, global::System.String name)
        {
            User user = new User();
            user.Token = token;
            user.Name = name;
            return user;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Token
        {
            get
            {
                return _Token;
            }
            set
            {
                if (_Token != value)
                {
                    OnTokenChanging(value);
                    ReportPropertyChanging("Token");
                    _Token = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("Token");
                    OnTokenChanged();
                }
            }
        }
        private global::System.String _Token;
        partial void OnTokenChanging(global::System.String value);
        partial void OnTokenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();

        #endregion
    
    }

    #endregion
    
}
