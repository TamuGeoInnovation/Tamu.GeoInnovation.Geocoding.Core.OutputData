using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TAMU.GeoInnovation.PointIntersectors.Census.OutputData.CensusRecords;
using USC.GISResearchLab.Census.Core.Configurations.ServerConfigurations;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.GeocodingQueries;
using USC.GISResearchLab.Common.Core.Geocoders.GeocodingQueries.Options;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Geometries.Polygons;
using USC.GISResearchLab.Common.Utils.Strings;
using USC.GISResearchLab.Core.WebServices.ResultCodes;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchingMethods;
using USC.GISResearchLab.Geocoding.Core.Algorithms.TieHandlingMethods;
using USC.GISResearchLab.Geocoding.Core.Metadata;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureMatchingResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Utils.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.OutputData
{
    public interface IGeocode
    {

        #region Properties

        string GeocoderName { get; set; }
        string RecordId { get; set; }
        DateTime Created { get; set; }
        double Version { get; set; }
        Version VersionNew { get; set; }

        string TransactionId { get; set; }
        string Resultstring { get; set; }
        QueryStatusCodes QueryStatusCodes { get; set; }
        bool Attempted { get; set; }
        bool Valid { get; set; }
        bool PreParsed { get; set; }

        string SourceType { get; set; }
        string SourceVintage { get; set; }
        string MethodType { get; set; }

        bool ExceptionOccurred { get; set; }
        string ErrorMessage { get; set; }

        [XmlIgnore]
        Exception Exception { get; set; }
        string SourceError { get; set; }
        string MethodError { get; set; }

        TimeSpan TimeTaken { get; set; }
        TimeSpan TimeTakenMatching { get; set; }
        TimeSpan TimeTakenInterpolation { get; set; }
        TimeSpan TimeTakenCensusIntersection { get; set; }
        TimeSpan TotalTimeTaken { get; set; }

        FeatureMatchingResult FM_Result { get; set; }
        FeatureMatchingResultType FM_ResultType { get; set; }
        string FM_Notes { get; set; }
        string FM_TieNotes { get; set; }
        int FM_ResultCount { get; set; }
        TieHandlingStrategyType FM_TieStrategy { get; set; }
        FeatureMatchingGeographyType FM_GeographyType { get; set; }

        GeocodeQualityType GeocodeQualityType { get; set; }

        string MicroMatchStatus { get; set; }

        string NAACCRGISCoordinateQualityCode { get; set; }
        string NAACCRGISCoordinateQualityName { get; set; }
        NAACCRGISCoordinateQualityType NAACCRGISCoordinateQualityType { get; set; }

        string NAACCRCensusTractCertaintyCode { get; set; }
        string NAACCRCensusTractCertaintyName { get; set; }
        NAACCRCensusTractCertaintyType NAACCRCensusTractCertaintyType { get; set; }

        InterpolationType InterpolationType { get; set; }
        InterpolationSubType InterpolationSubType { get; set; }

        string RegionSize { get; set; }
        string RegionSizeUnits { get; set; }

        FeatureMatchingSelectionMethod FM_SelectionMethod { get; set; }
        string FM_SelectionNotes { get; set; }

        GeocodeStatistics Statistics { get; set; }
        GeocodeStatistics CompleteProcessStatistics { get; set; }
        Geometry Geometry { get; set; }
        GeocodedError GeocodedError { get; set; }
        StreetAddress ParsedAddress { get; set; }
        StreetAddress InputAddress { get; set; }
        RelaxableStreetAddress MatchedAddress { get; set; }
        RelaxableStreetAddress[] MatchedAddresses { get; set; }
        StreetAddress MatchedFeatureAddress { get; set; }
        List<StreetAddress> MatchedFeatureAddresses { get; set; }
        MatchedFeature MatchedFeature { get; set; }
        List<MatchedFeature> MatchedFeatures { get; set; }
        GeocodingQuery GeocodingQuery { get; set; }

        List<CensusOutputRecord> CensusRecords { get; set; }
        CensusYear CensusYear { get; set; }
        double CensusTimeTaken { get; set; }
        string CensusStateFips { get; set; }
        string CensusCountyFips { get; set; }
        string CensusTract { get; set; }
        string CensusBlockGroup { get; set; }
        string CensusBlock { get; set; }
        string CensusPlaceFips { get; set; }
        string CensusMcdFips { get; set; }
        string CensusMsaFips { get; set; }
        string CensusMetDivFips { get; set; }
        string CensusCbsaFips { get; set; }
        string CensusCbsaMicro { get; set; }

        double CensusStateFipsTimeTaken { get; set; }
        double CensusCountyFipsTimeTaken { get; set; }
        double CensusTractTimeTaken { get; set; }
        double CensusBlockGroupTimeTaken { get; set; }
        double CensusBlockTimeTaken { get; set; }
        double CensusPlaceFipsTimeTaken { get; set; }
        double CensusMcdFipsTimeTaken { get; set; }
        double CensusMsaFipsTimeTaken { get; set; }
        double CensusMetDivFipsTimeTaken { get; set; }
        double CensusCbsaFipsTimeTaken { get; set; }
        double CensusCbsaMicroTimeTaken { get; set; }

        bool CensusExceptionOccured { get; set; }
        string CensusExceptionMessage { get; set; }
        Exception CensusException { get; set; }

        string Quality { get; }


        bool MatchScoreSet { get; set; }

        double MatchScore { get; set; }

        string MatchedLocationType { get; }

        string MatchType { get; }
        string CoordinateString { get; }
        double Latitude { get; }
        double Longitude { get; }
        #endregion



        void SetQueryStatusCode(int queryStatusCodeValue);

        void SetQueryStatusCode(QueryStatusCodes queryStatusCode);

        void SetMatchedLocationType(int matchedLocationType);

        void SetMatchedLocationType(string matchedLocationTypeName);

        void SetMatchedLocationType(MatchedLocationTypes matchedLocationType);

        void SetMatchType(string matchType);

        string GetAnyErrors();


        string ToString(bool verbose);

        string AsStringVerbose_V03_01(string separator, double version, BatchOptions options);

    }
}