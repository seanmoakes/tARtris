//-----------------------------------------------------------------------
// <copyright file="TrackableHitFlag.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore
{
    using UnityEngine;

    /// <summary>
    /// Flags representing categories of raycast hits.
    /// </summary>
    [System.Flags]
    public enum TrackableHitFlag
    {
        /// <summary>
        /// This value is returned on a TrackableHit to indicate no collision occurred.
        ///
        /// If you pass this into Raycast, you will not get any collision results.
        /// </summary>
        None = 0,

        /// <summary>
        /// The collision is within the the TrackedPlane's convex bounding polygon.
        /// </summary>
        PlaneWithinPolygon = 1,

        /// <summary>
        /// The collision is within the TrackedPlane's bounding box.
        /// </summary>
        PlaneWithinBounds = 2,

        /// <summary>
        /// The collision is on the TrackedPlane, but not limited to the bounding box or polygon.
        /// This acts as if the plane extends out to infinity.
        /// </summary>
        PlaneWithinInfinity = 4,

        /// <summary>
        /// The collision is on points in the current frame's point cloud.
        /// </summary>
        PointCloud = 8,
    }
}
