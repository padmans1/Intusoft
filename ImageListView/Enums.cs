﻿// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)

using System;
using System.Windows.Forms;

namespace INTUSOFT.ImageListView
{
    #region Public Enums
    /// <summary>
    /// Represents the cache mode.
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        /// Item thumbnails will be generated only when requested.
        /// </summary>
        OnDemand,
        /// <summary>
        /// Item thumbnails will be continuously generated. Setting
        /// the CacheMode to Continuous disables the CacheLimit.
        /// </summary>
        Continuous,
    }
    /// <summary>
    /// Represents the cache state of a thumbnail image.
    /// </summary>
    public enum CacheState
    {
        /// <summary>
        /// The item is either not cached or it is in the cache queue.
        /// </summary>
        Unknown,
        /// <summary>
        /// Item thumbnail is cached.
        /// </summary>
        Cached,
        /// <summary>
        /// An error occurred while creating the item thumbnail.
        /// </summary>
        Error,
    }
    /// <summary>
    /// Represents the visual state of an image list column.
    /// </summary>
    [Flags]
    public enum ColumnState
    {
        /// <summary>
        /// The column is not hovered.
        /// </summary>
        None = 0,
        /// <summary>
        /// Mouse cursor is over the column.
        /// </summary>
        Hovered = 1,
        /// <summary>
        /// Mouse cursor is over the column separator.
        /// </summary>
        SeparatorHovered = 2,
        /// <summary>
        /// Column separator is being dragged.
        /// </summary>
        SeparatorSelected = 4,
    }
    /// <summary>
    /// Represents the type of information displayed in an image list view column.
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// The text of the item, defaults to filename if
        /// the text is not provided.
        /// </summary>
        Name,
        /// <summary>
        /// The last access date.
        /// </summary>
        DateAccessed,
        /// <summary>
        /// The creation date.
        /// </summary>
        DateCreated,
        /// <summary>
        /// The last modification date.
        /// </summary>
        DateModified,
        /// <summary>
        /// Mime type of the file.
        /// </summary>
        FileType,
        /// <summary>
        /// The full path to the file.
        /// </summary>
        FileName,
        /// <summary>
        /// The path to the folder containing the file.
        /// </summary>
        FilePath,
        /// <summary>
        /// The size of the file.
        /// </summary>
        FileSize,
        /// <summary>
        /// Image dimensions in pixels.
        /// </summary>
        Dimensions,
        /// <summary>
        /// Image resolution if dpi.
        /// </summary>
        Resolution,
        /// <summary>
        /// Image description (Exif tag).
        /// </summary>
        ImageDescription,
        /// <summary>
        /// The equipment model (Exif tag).
        /// </summary>
        EquipmentModel,
        /// <summary>
        /// The date image was taken (Exif tag).
        /// </summary>
        DateTaken,
        /// <summary>
        /// The artist taking the image (Exif tag).
        /// </summary>
        Artist,
        /// <summary>
        /// Image copyright information (Exif tag).
        /// </summary>
        Copyright,
        /// <summary>
        /// Exposure time in seconds (Exif tag).
        /// </summary>
        ExposureTime,
        /// <summary>
        /// The F number (Exif tag).
        /// </summary>
        FNumber,
        /// <summary>
        /// ISO speed (Exif tag).
        /// </summary>
        ISOSpeed,
        /// <summary>
        /// Shutter speed (Exif tag).
        /// </summary>
        ShutterSpeed,
        /// <summary>
        /// The lens aperture (Exif tag).
        /// </summary>
        Aperture,
        /// <summary>
        /// User comment (Exif tag).
        /// </summary>
        UserComment,
    }
    /// <summary>
    /// Represents the order by which items are drawn.
    /// </summary>
    public enum ItemDrawOrder
    {
        /// <summary>
        /// Draw order is determined by item insertion index.
        /// </summary>
        ItemIndex,
        /// <summary>
        /// Draw order is determined by the ZOrder properties of items.
        /// </summary>
        ZOrder,
        /// <summary>
        /// Hovered items are drawn first, followed by normal items and selected items.
        /// </summary>
        HoveredNormalSelected,
        /// <summary>
        /// Hovered items are drawn first, followed by selected items and normal items.
        /// </summary>
        HoveredSelectedNormal,
        /// <summary>
        /// Normal items are drawn first, followed by hovered items and selected items.
        /// </summary>
        NormalHoveredSelected,
        /// <summary>
        /// Normal items are drawn first, followed by selected items and hovered items.
        /// </summary>
        NormalSelectedHovered,
        /// <summary>
        /// Selected items are drawn first, followed by hovered items and normal items.
        /// </summary>
        SelectedHoveredNormal,
        /// <summary>
        /// Selected items are drawn first, followed by normal items and hovered items.
        /// </summary>
        SelectedNormalHovered,
    }
    /// <summary>
    /// Represents the visual state of an image list view item.
    /// </summary>
    [Flags]
    public enum ItemState
    {
        /// <summary>
        /// The item is neither selected nor hovered.
        /// </summary>
        None = 0,
        /// <summary>
        /// The item is selected.
        /// </summary>
        Selected = 1,
        /// <summary>
        /// The item has the input focus.
        /// </summary>
        Focused = 2,
        /// <summary>
        /// Mouse cursor is over the item.
        /// </summary>
        Hovered = 4,
    }
    /// <summary>
    /// Determines the visibility of an item.
    /// </summary>
    public enum ItemVisibility
    {
        /// <summary>
        /// The item is not visible.
        /// </summary>
        NotVisible,
        /// <summary>
        /// The item is partially visible.
        /// </summary>
        PartiallyVisible,
        /// <summary>
        /// The item is fully visible.
        /// </summary>
        Visible,
    }
    /// <summary>
    /// Represents the embedded thumbnail extraction behavior.
    /// </summary>
    public enum UseEmbeddedThumbnails
    {
        /// <summary>
        /// Always creates the thumbnail from the embedded thumbnail.
        /// </summary>
        Always,
        /// <summary>
        /// Creates the thumbnail from the embedded thumbnail when possible,
        /// reverts to the source image otherwise.
        /// </summary>
        Auto,
        /// <summary>
        /// Always creates the thumbnail from the source image.
        /// </summary>
        Never,
    }
    /// <summary>
    /// Represents the view mode of the image list view.
    /// </summary>
    public enum View
    {
        /// <summary>
        /// Displays columns with image details. Thumbnail images
        /// are not displayed. The view can be scrolled vertically.
        /// </summary>
        Details,
        /// <summary>
        /// Displays a single row of thumbnails at the bottom.
        /// The view can be scrolled horizontally.
        /// </summary>
        Gallery,
        /// <summary>
        /// Displays a pane with item details.The view can be 
        /// scrolled vertically.
        /// </summary>
        Pane,
        /// <summary>
        /// Displays thumbnails laid out in a grid. The view can be 
        /// scrolled vertically.
        /// </summary>
        Thumbnails,
    }
    #endregion

    #region Internal Enums
    /// <summary>
    /// Represents the item highlight state during mouse selection.
    /// </summary>
    internal enum ItemHighlightState
    {
        /// <summary>
        /// Item is not highlighted.
        /// </summary>
        NotHighlighted,
        /// <summary>
        /// Item is highlighted and will be removed from the selection set.
        /// </summary>
        HighlightedAndUnSelected,
        /// <summary>
        /// Item is highlighted and will be added to the selection set.
        /// </summary>
        HighlightedAndSelected,
    }
    #endregion
}
