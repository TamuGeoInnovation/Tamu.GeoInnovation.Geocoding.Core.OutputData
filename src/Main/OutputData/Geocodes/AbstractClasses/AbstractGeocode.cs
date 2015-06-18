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
    public abstract class AbstractGeocode : IGeocode
    {

        #region Properties

        public string GeocoderName { get; set; }
        public  string RecordId { get; set; }
        public DateTime Created { get; set; }
        public double Version { get; set; }

        public string TransactionId { get; set; }
        public string Resultstring { get; set; }
        public QueryStatusCodes QueryStatusCodes { get; set; }
        public bool Attempted { get; set; }
        public bool Valid { get; set; }
        public bool PreParsed { get; set; }
        
        public string SourceType { get; set; }
        public string SourceVintage { get; set; }
        public string MethodType { get; set; }

        public bool ExceptionOccurred { get; set; }
        public string ErrorMessage { get; set; }

        [XmlIgnore]
        public Exception Exception { get; set; }
        public string SourceError { get; set; }
        public string MethodError { get; set; }

        public TimeSpan TimeTaken { get; set; }
        public TimeSpan TimeTakenMatching { get; set; }
        public TimeSpan TimeTakenInterpolation { get; set; }
        public TimeSpan TimeTakenCensusIntersection { get; set; }
        public TimeSpan TotalTimeTaken { get; set; }

        public FeatureMatchingResult FM_Result { get; set; }
        public FeatureMatchingResultType FM_ResultType { get; set; }
        public string FM_Notes { get; set; }
        public string FM_TieNotes { get; set; }
        public int FM_ResultCount { get; set; }
        public TieHandlingStrategyType FM_TieStrategy { get; set; }
        public FeatureMatchingGeographyType FM_GeographyType { get; set; }

        public GeocodeQualityType GeocodeQualityType { get; set; }

        public string NAACCRGISCoordinateQualityCode { get; set; }
        public string NAACCRGISCoordinateQualityName { get; set; }
        public NAACCRGISCoordinateQualityType NAACCRGISCoordinateQualityType { get; set; }

        public string NAACCRCensusTractCertaintyCode { get; set; }
        public string NAACCRCensusTractCertaintyName { get; set; }
        public NAACCRCensusTractCertaintyType NAACCRCensusTractCertaintyType { get; set; }

        public InterpolationType InterpolationType { get; set; }
        public InterpolationSubType InterpolationSubType { get; set; }

        public string RegionSize { get; set; }
        public string RegionSizeUnits { get; set; }

        public FeatureMatchingSelectionMethod FM_SelectionMethod { get; set; }
        public string FM_SelectionNotes { get; set; }

        public GeocodeStatistics Statistics { get; set; }
        public GeocodeStatistics CompleteProcessStatistics { get; set; }
        public Geometry Geometry { get; set; }
        public GeocodedError GeocodedError { get; set; }
        public StreetAddress ParsedAddress { get; set; }
        public StreetAddress InputAddress { get; set; }
        public RelaxableStreetAddress MatchedAddress { get; set; }
        public RelaxableStreetAddress[] MatchedAddresses { get; set; }
        public StreetAddress MatchedFeatureAddress { get; set; }
        public List<StreetAddress> MatchedFeatureAddresses { get; set; }
        public MatchedFeature MatchedFeature { get; set; }
        public List<MatchedFeature> MatchedFeatures { get; set; }
        public GeocodingQuery GeocodingQuery { get; set; }

        public List<CensusOutputRecord> CensusRecords {get;set;}
        public CensusYear CensusYear { get; set; }
        public double CensusTimeTaken { get; set; }
        public string CensusStateFips { get; set; }
        public string CensusCountyFips { get; set; }
        public string CensusTract { get; set; }
        public string CensusBlockGroup { get; set; }
        public string CensusBlock { get; set; }
        public string CensusPlaceFips { get; set; }
        public string CensusMcdFips { get; set; }
        public string CensusMsaFips { get; set; }
        public string CensusMetDivFips { get; set; }
        public string CensusCbsaFips { get; set; }
        public string CensusCbsaMicro { get; set; }

        public double CensusStateFipsTimeTaken { get; set; }
        public double CensusCountyFipsTimeTaken { get; set; }
        public double CensusTractTimeTaken { get; set; }
        public double CensusBlockGroupTimeTaken { get; set; }
        public double CensusBlockTimeTaken { get; set; }
        public double CensusPlaceFipsTimeTaken { get; set; }
        public double CensusMcdFipsTimeTaken { get; set; }
        public double CensusMsaFipsTimeTaken { get; set; }
        public double CensusMetDivFipsTimeTaken { get; set; }
        public double CensusCbsaFipsTimeTaken { get; set; }
        public double CensusCbsaMicroTimeTaken { get; set; }

        public  bool CensusExceptionOccured { get; set; }
        public string CensusExceptionMessage { get; set; }
        public Exception CensusException { get; set; }

        
        public bool MatchScoreSet { get; set; }
        public double MatchScore { get; set; }

        private string _Quality;
        public string Quality
        {
            get
            {
                return _Quality;
            }
        }

        private string _MatchedLocationType;
        public string MatchedLocationType
        {
            get
            {
                return _MatchedLocationType;
            }
        }

        private string _MatchType;
        public string MatchType
        {
            get
            {
                return _MatchType;
            }
        }

        private string _CoordinateString;
        public string CoordinateString
        {
            get
            {
                return _CoordinateString;
            }
        }

        private double _Latitude;
        public double Latitude
        {
            get
            {
                return _Latitude;
            }
        }

        private double _Longitude;
        public double Longitude
        {
            get
            {
                return _Longitude;
            }
        }

        #endregion

        //public Geocode()
        //    : this(new AddressNormalizer().AddressParserVersion)
        //{

        //}

        public AbstractGeocode(double version)
            : this()
        {
            Version = version;
        }

        public AbstractGeocode()
        {
           
            Statistics = new GeocodeStatistics();
            GeocodedError = new GeocodedError();
            QueryStatusCodes = QueryStatusCodes.Unknown;

            Geometry = new Point();
            MatchedAddress = new RelaxableStreetAddress();
            MatchedFeature = new MatchedFeature();
            MatchedFeatureAddress = new StreetAddress();
            ParsedAddress = new StreetAddress();
            FM_Result = new FeatureMatchingResult();

            Valid = false;

            Created = DateTime.Now;

            SourceType = "";
            SourceVintage = "";
            SourceError = "";
            ErrorMessage = "";

            GeocodeQualityType = GeocodeQualityType.Unmatchable;
            FM_ResultType = FeatureMatchingResultType.Unmatchable;
            InterpolationSubType = InterpolationSubType.NotAttempted;
            InterpolationType = InterpolationType.NotAttempted;
            FM_GeographyType = FeatureMatchingGeographyType.Unmatchable;

            FM_SelectionNotes = "";
            FM_Notes = "";
            FM_TieNotes = "";

            NAACCRGISCoordinateQualityCode = "";
            NAACCRGISCoordinateQualityName = "";
            NAACCRGISCoordinateQualityType = NAACCRGISCoordinateQualityType.Unknown;

            NAACCRCensusTractCertaintyCode = "";
            NAACCRCensusTractCertaintyName = "";
            NAACCRCensusTractCertaintyType = NAACCRCensusTractCertaintyType.Unknown;
        }

       

        public void SetQueryStatusCode(int queryStatusCodeValue)
        {
            QueryStatusCodes = QueryResultCodeManager.GetStatusCodeFromValue(queryStatusCodeValue);
        }

        public void SetQueryStatusCode(QueryStatusCodes queryStatusCode)
        {
            QueryStatusCodes = queryStatusCode;
        }

        public virtual void SetMatchedLocationType(int matchedLocationType)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationType);
        }

        public virtual void SetMatchedLocationType(string matchedLocationTypeName)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationTypeName);
        }

        public virtual void SetMatchedLocationType(MatchedLocationTypes matchedLocationType)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationType);
        }

        public virtual void SetMatchType(string matchType)
        {
            if (MatchedFeature == null)
            {
                MatchedFeature = new MatchedFeature();
            }

            if (!String.IsNullOrEmpty(matchType ))
            {
                string[] parts = null;
                if (matchType.IndexOf(',') >= 0)
                {
                    parts = matchType.Split(',');
                }
                else if (matchType.IndexOf(';') >= 0)
                {
                    parts = matchType.Split(';');
                }
                else
                {
                    parts = new string[1];
                    parts[0] = matchType;
                }

                if (parts != null)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string part = parts[i];

                        if (String.Compare(part, FeatureMatcher.FEATURE_MATCH_TYPE_NAME_COMPOSITE, true) == 0)
                        {
                            MatchedFeature.IsCompositeMatch = true;
                        }
                        else if (String.Compare(part, FeatureMatcher.FEATURE_MATCH_TYPE_NAME_EXACT, true) == 0)
                        {
                            MatchedFeature.IsExactMatch = true;
                        }
                        else if (String.Compare(part, FeatureMatcher.FEATURE_MATCH_TYPE_NAME_RELAXED, true) == 0)
                        {
                            MatchedFeature.IsRelaxedMatch = true;
                        }
                        else if (String.Compare(part, FeatureMatcher.FEATURE_MATCH_TYPE_NAME_SOUNDEX, true) == 0)
                        {
                            MatchedFeature.IsSoundexMatch = true;
                        }
                        else if (String.Compare(part, FeatureMatcher.FEATURE_MATCH_TYPE_NAME_SUBSTRING, true) == 0)
                        {
                            MatchedFeature.IsSubstringMatch = true;
                        }
                    }
                }
            }
        }

        public string GetAnyErrors()
        {
            string ret = "";
            if (!String.IsNullOrEmpty(MethodError))
            {
                ret += "Method Error: " + MethodError;
            }

            if (!String.IsNullOrEmpty(SourceError))
            {
                if (!String.IsNullOrEmpty(MethodError))
                {
                    ret += "|";
                }

                ret += "Source Error: " + SourceError;
            }

            return ret;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public virtual string ToString(bool verbose)
        {
            StringBuilder ret = new StringBuilder();
            if (!verbose)
            {
                if (Geometry != null)
                {
                    ret.AppendFormat("{0}", GeocodeQualityType.ToString());
                    ret.Append(",");
                    ret.AppendFormat("{0}", Statistics.MatchedLocationTypeStatistics.MatchedLocationTypeIndex);
                    ret.Append(",");
                    ret.Append(Geometry.ToString());
                    ret.AppendFormat(", {0}, {1}, {2}, {3}", SourceType, SourceError, MethodType, MethodError);
                    if (GeocodedError.GeoError != null)
                    {
                        ret.AppendFormat(",{0}", GeocodedError.GeoError);
                        ret.AppendFormat(",{0} {1}, {2}", GeocodedError.ErrorBounds, UnitManager.FromLinearUnitType(GeocodedError.ErrorBoundsUnit), GeocodedErrorCalculationTypes.NameFromType(GeocodedError.ErrorCalculationType));
                    }
                }
                else
                {
                    throw new Exception("Geometry is null in a geocode, this should never be the case");
                }

                if (ParsedAddress != null)
                {
                    ret.AppendFormat(",{0}", ParsedAddress.ToString());
                }
                else
                {
                    throw new Exception("ParsedAddress is null in a geocode, this should never be the case");
                }
            }
            else
            {

                if (Geometry != null)
                {
                    ret.AppendLine(Geometry.ToString());
                }
                else
                {
                    throw new Exception("Geometry is null in a geocode, this should never be the case");
                }
                ret.AppendLine(GeocodedError.ToString());
                ret.AppendLine(Statistics.ToString());
            }
            return ret.ToString();
        }

        public virtual string AsStringVerbose_V03_01(string separator, double version, BatchOptions options)
        {
            StringBuilder sb = new StringBuilder();


            sb.Append("\"" + StringUtils.EscapeChar(Latitude.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //3
            sb.Append("\"" + StringUtils.EscapeChar(Longitude.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator);  //4
            sb.Append("\"" + StringUtils.EscapeChar(NAACCRGISCoordinateQualityCode, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator);  //5
            sb.Append("\"" + StringUtils.EscapeChar(NAACCRGISCoordinateQualityType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //6
            sb.Append("\"" + StringUtils.EscapeChar(MatchScore.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //7
            sb.Append("\"" + StringUtils.EscapeChar(MatchType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //8
            sb.Append("\"" + StringUtils.EscapeChar(FM_GeographyType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //9
            sb.Append("\"" + StringUtils.EscapeChar(RegionSize, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //10
            sb.Append("\"" + StringUtils.EscapeChar(RegionSizeUnits, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //11
            sb.Append("\"" + StringUtils.EscapeChar(InterpolationType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //12
            sb.Append("\"" + StringUtils.EscapeChar(InterpolationSubType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //13
            sb.Append("\"" + StringUtils.EscapeChar(MatchedLocationType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //14
            sb.Append("\"" + StringUtils.EscapeChar(FM_ResultType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //15
            sb.Append("\"" + StringUtils.EscapeChar(FM_ResultCount.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //16
            sb.Append("\"" + StringUtils.EscapeChar(FM_Notes, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //17
            sb.Append("\"" + StringUtils.EscapeChar(FM_TieNotes, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //18
            sb.Append("\"" + StringUtils.EscapeChar(FM_TieStrategy.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //19
            sb.Append("\"" + StringUtils.EscapeChar(FM_SelectionMethod.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //20
            sb.Append("\"" + StringUtils.EscapeChar(FM_SelectionNotes, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21

            if (options == null || options.OutputBookKeepingFieldMappings.Enabled)
            {
                sb.Append("\"" + StringUtils.EscapeChar(Version.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(QueryStatusCodes.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //2
                sb.Append("\"" + StringUtils.EscapeChar(TimeTaken.TotalMilliseconds.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(TransactionId, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(GeocoderName, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(ErrorMessage, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(1.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
                sb.Append("\"" + StringUtils.EscapeChar(RecordId, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //21
            }


            if (options == null || options.OutputCensusFieldMappings.Enabled)
            {
                sb.Append("\"" + StringUtils.EscapeChar(CensusYear.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //23
                sb.Append("\"" + StringUtils.EscapeChar(NAACCRCensusTractCertaintyCode, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //24
                sb.Append("\"" + StringUtils.EscapeChar(NAACCRCensusTractCertaintyType.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //25
                sb.Append("\"" + StringUtils.EscapeChar(CensusBlock, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //26
                sb.Append("\"" + StringUtils.EscapeChar(CensusBlockGroup, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //27
                sb.Append("\"" + StringUtils.EscapeChar(CensusTract, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //28
                sb.Append("\"" + StringUtils.EscapeChar(CensusCountyFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //29
                sb.Append("\"" + StringUtils.EscapeChar(CensusCbsaFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //30
                sb.Append("\"" + StringUtils.EscapeChar(CensusCbsaMicro, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //31
                sb.Append("\"" + StringUtils.EscapeChar(CensusMcdFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //32
                sb.Append("\"" + StringUtils.EscapeChar(CensusMetDivFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //33
                sb.Append("\"" + StringUtils.EscapeChar(CensusMsaFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //34
                sb.Append("\"" + StringUtils.EscapeChar(CensusPlaceFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //35
                sb.Append("\"" + StringUtils.EscapeChar(CensusStateFips, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //36
            }

            if (options == null || options.OutputInputAddressFieldMappings.Enabled)
            {
                StreetAddress address = InputAddress;
                sb.Append("\"" + StringUtils.EscapeChar(address.Number, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //37
                sb.Append("\"" + StringUtils.EscapeChar(address.NumberFractional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //38
                sb.Append("\"" + StringUtils.EscapeChar(address.PreDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //39
                sb.Append("\"" + StringUtils.EscapeChar(address.PreQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //40
                sb.Append("\"" + StringUtils.EscapeChar(address.PreType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //41
                sb.Append("\"" + StringUtils.EscapeChar(address.PreArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //42
                sb.Append("\"" + StringUtils.EscapeChar(address.StreetName, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //43
                sb.Append("\"" + StringUtils.EscapeChar(address.PostArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //44
                sb.Append("\"" + StringUtils.EscapeChar(address.PostQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //45
                sb.Append("\"" + StringUtils.EscapeChar(address.Suffix, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //46
                sb.Append("\"" + StringUtils.EscapeChar(address.PostDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //47
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //48
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //49
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //50
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //51
                sb.Append("\"" + StringUtils.EscapeChar(address.City, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //52
                sb.Append("\"" + StringUtils.EscapeChar(address.ConsolidatedCity, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //53
                sb.Append("\"" + StringUtils.EscapeChar(address.MinorCivilDivision, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //54
                sb.Append("\"" + StringUtils.EscapeChar(address.CountySubregion, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //55
                sb.Append("\"" + StringUtils.EscapeChar(address.County, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //56
                sb.Append("\"" + StringUtils.EscapeChar(address.State, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //57
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIP, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //58
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus1, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //59
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus2, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //60
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus3, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //61
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus4, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //62
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus5, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //63
            }

            if (options == null || options.OutputMatchedAddressFieldMappings.Enabled)
            {
                StreetAddress address = MatchedAddress;
                sb.Append("\"" + StringUtils.EscapeChar(address.Number, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //37
                sb.Append("\"" + StringUtils.EscapeChar(address.NumberFractional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //38
                sb.Append("\"" + StringUtils.EscapeChar(address.PreDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //39
                sb.Append("\"" + StringUtils.EscapeChar(address.PreQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //40
                sb.Append("\"" + StringUtils.EscapeChar(address.PreType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //41
                sb.Append("\"" + StringUtils.EscapeChar(address.PreArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //42
                sb.Append("\"" + StringUtils.EscapeChar(address.StreetName, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //43
                sb.Append("\"" + StringUtils.EscapeChar(address.PostArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //44
                sb.Append("\"" + StringUtils.EscapeChar(address.PostQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //45
                sb.Append("\"" + StringUtils.EscapeChar(address.Suffix, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //46
                sb.Append("\"" + StringUtils.EscapeChar(address.PostDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //47
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //48
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //49
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //50
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //51
                sb.Append("\"" + StringUtils.EscapeChar(address.City, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //52
                sb.Append("\"" + StringUtils.EscapeChar(address.ConsolidatedCity, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //53
                sb.Append("\"" + StringUtils.EscapeChar(address.MinorCivilDivision, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //54
                sb.Append("\"" + StringUtils.EscapeChar(address.CountySubregion, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //55
                sb.Append("\"" + StringUtils.EscapeChar(address.County, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //56
                sb.Append("\"" + StringUtils.EscapeChar(address.State, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //57
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIP, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //58
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus1, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //59
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus2, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //60
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus3, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //61
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus4, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //62
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus5, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //63
            }

            if (options == null || options.OutputParsedAddressFieldMappings.Enabled)
            {
                StreetAddress address = ParsedAddress;
                sb.Append("\"" + StringUtils.EscapeChar(address.Number, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //64
                sb.Append("\"" + StringUtils.EscapeChar(address.NumberFractional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //65
                sb.Append("\"" + StringUtils.EscapeChar(address.PreDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //66
                sb.Append("\"" + StringUtils.EscapeChar(address.PreQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //67
                sb.Append("\"" + StringUtils.EscapeChar(address.PreType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //68
                sb.Append("\"" + StringUtils.EscapeChar(address.PreArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //69
                sb.Append("\"" + StringUtils.EscapeChar(address.StreetName, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //70
                sb.Append("\"" + StringUtils.EscapeChar(address.PostArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //71
                sb.Append("\"" + StringUtils.EscapeChar(address.PostQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //72
                sb.Append("\"" + StringUtils.EscapeChar(address.Suffix, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //73
                sb.Append("\"" + StringUtils.EscapeChar(address.PostDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //74
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //75
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //76
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //77
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //78
                sb.Append("\"" + StringUtils.EscapeChar(address.City, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //79
                sb.Append("\"" + StringUtils.EscapeChar(address.ConsolidatedCity, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //80
                sb.Append("\"" + StringUtils.EscapeChar(address.MinorCivilDivision, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //81
                sb.Append("\"" + StringUtils.EscapeChar(address.CountySubregion, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //82
                sb.Append("\"" + StringUtils.EscapeChar(address.County, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //83
                sb.Append("\"" + StringUtils.EscapeChar(address.State, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //84
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIP, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //85
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus1, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //86
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus2, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //87
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus3, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //88
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus4, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //89
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus5, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //90
            }

            if (options == null || options.OutputReferenceFeatureFieldMappings.Enabled)
            {
                StreetAddress address = MatchedFeatureAddress;
                sb.Append("\"" + StringUtils.EscapeChar(address.Number, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //91
                sb.Append("\"" + StringUtils.EscapeChar(address.NumberFractional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //92
                sb.Append("\"" + StringUtils.EscapeChar(address.PreDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //93
                sb.Append("\"" + StringUtils.EscapeChar(address.PreQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //94
                sb.Append("\"" + StringUtils.EscapeChar(address.PreType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //95
                sb.Append("\"" + StringUtils.EscapeChar(address.PreArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //96
                sb.Append("\"" + StringUtils.EscapeChar(address.StreetName, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //97
                sb.Append("\"" + StringUtils.EscapeChar(address.PostArticle, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //98
                sb.Append("\"" + StringUtils.EscapeChar(address.PostQualifier, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //99
                sb.Append("\"" + StringUtils.EscapeChar(address.Suffix, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //100
                sb.Append("\"" + StringUtils.EscapeChar(address.PostDirectional, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //101
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //102
                sb.Append("\"" + StringUtils.EscapeChar(address.SuiteNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //103
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //104
                sb.Append("\"" + StringUtils.EscapeChar(address.PostOfficeBoxNumber, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //105
                sb.Append("\"" + StringUtils.EscapeChar(address.City, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //106
                sb.Append("\"" + StringUtils.EscapeChar(address.ConsolidatedCity, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //107
                sb.Append("\"" + StringUtils.EscapeChar(address.MinorCivilDivision, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //108
                sb.Append("\"" + StringUtils.EscapeChar(address.CountySubregion, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //109
                sb.Append("\"" + StringUtils.EscapeChar(address.County, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //110
                sb.Append("\"" + StringUtils.EscapeChar(address.State, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //111
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIP, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //112
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus1, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //113
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus2, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //114
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus3, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //115
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus4, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //116
                sb.Append("\"" + StringUtils.EscapeChar(address.ZIPPlus5, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //117
                
                if (MatchedFeature.MatchedReferenceFeature != null)
                {
                    sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); ; //118
                    sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); ; //119
                }
                else
                {
                    sb.Append("\"\"").Append(separator); ; //118
                    sb.Append("\"\"").Append(separator); ; //119
                }

                string gml = "";
                int srid = 0;
                if (MatchedFeature != null && MatchedFeature.MatchedReferenceFeature != null)
                {
                    if (MatchedFeature != null && MatchedFeature.MatchedReferenceFeature != null)
                    {
                        if (MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry != null)
                        {
                            if (MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry != null && !MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.IsNull)
                            {
                                if (options.ShouldOutputReferenceFeatureGeometry)
                                {
                                    srid = MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SRID;
                                    gml = MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.AsGml().Value;
                                }
                            }
                        }
                    }
                }


                sb.Append("\"" + StringUtils.EscapeChar(srid.ToString(), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //120

                if (String.Compare(separator, ",", true) == 0)
                {
                    sb.Append("\"" + StringUtils.EscapeChar(gml.Replace(separator, ";"), "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //121
                }
                else
                {
                    sb.Append("\"" + StringUtils.EscapeChar(gml, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //121
                }

                sb.Append("\"" + StringUtils.EscapeChar(SourceType, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //122
                sb.Append("\"" + StringUtils.EscapeChar(SourceVintage, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //123
                sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.PrimaryIdField, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //124
                sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.PrimaryIdValue, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //125
                sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.SecondaryIdField, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //126
                sb.Append("\"" + StringUtils.EscapeChar(MatchedFeature.SecondaryIdValue, "\"", StringUtils.EsacpeCharAction.repeat) + "\"").Append(separator); //127

               
            }

            return sb.ToString();
        }



        public static string AsHeaderStringVerbose_V03_01(string separator, double version, BatchOptions options)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("Latitude").Append(separator); //3
            sb.Append("Longitude").Append(separator);  //4
            sb.Append("NAACCRGISCoordinateQualityCode").Append(separator);  //5
            sb.Append("NAACCRGISCoordinateQualityName").Append(separator); //6
            sb.Append("MatchScore").Append(separator); //7
            sb.Append("MatchType").Append(separator); //8
            sb.Append("FeatureMatchingGeographyType").Append(separator); //9
            sb.Append("RegionSize").Append(separator); //10
            sb.Append("RegionSizeUncertainty").Append(separator); //11
            sb.Append("InterpolationType").Append(separator); //12
            sb.Append("InterpolationSubType").Append(separator); //13
            sb.Append("MatchedLocationType").Append(separator); //14
            sb.Append("FeatureMatchingResultType").Append(separator); //15
            sb.Append("FeatureMatchingResultCount").Append(separator); //16
            sb.Append("FeatureMatchingResultTypeNotes").Append(separator); //17
            sb.Append("FeatureMatchingResultTypeTieBreakingNotes").Append(separator); //18
            sb.Append("TieHandlingStrategyType").Append(separator); //19
            sb.Append("FeatureMatchingSelectionMethod").Append(separator); //20
            sb.Append("FeatureMatchingSelectionMethodNotes").Append(separator); //21
           

            if (options == null || options.OutputBookKeepingFieldMappings.Enabled)
            {
                sb.Append("Version").Append(separator); //2
                sb.Append("QueryStatusCodeValue").Append(separator); //2
                sb.Append("TimeTaken").Append(separator); //2
                sb.Append("TransactionId").Append(separator); //2
                sb.Append("Source").Append(separator); //2
                sb.Append("ErrorMessage").Append(separator); //2
                sb.Append("Processed").Append(separator); //2
                sb.Append("RecordId").Append(separator); //2
            }

            if (options == null || options.OutputCensusFieldMappings.Enabled)
            {
                sb.Append("CensusYear").Append(separator); //23
                sb.Append("NAACCRCensusTractCertaintyCode").Append(separator); //24
                sb.Append("NAACCRCensusTractCertaintyName").Append(separator); //25
                sb.Append("CensusBlock").Append(separator); //26
                sb.Append("CensusBlockGroup").Append(separator); //27
                sb.Append("CensusTract").Append(separator); //28
                sb.Append("CensusCountyFips").Append(separator); //29
                sb.Append("CensusCbsaFips").Append(separator); //30
                sb.Append("CensusCbsaMicro").Append(separator); //31
                sb.Append("CensusMcdFips").Append(separator); //32
                sb.Append("CensusMetDivFips").Append(separator); //33
                sb.Append("CensusMsaFips").Append(separator); //34
                sb.Append("CensusPlaceFips").Append(separator); //35
                sb.Append("CensusStateFips").Append(separator); //36
            }

            if (options == null || options.OutputInputAddressFieldMappings.Enabled)
            {
                sb.Append("INumber").Append(separator); //37
                sb.Append("INumberFractional").Append(separator); //38
                sb.Append("IPreDirectional").Append(separator); //39
                sb.Append("IPreQualifier").Append(separator); //40
                sb.Append("IPreType").Append(separator); //41
                sb.Append("IPreArticle").Append(separator); //42
                sb.Append("IName").Append(separator); //43
                sb.Append("IPostArticle").Append(separator); //44
                sb.Append("IPostQualifier").Append(separator); //45
                sb.Append("ISuffix").Append(separator); //46
                sb.Append("IPostDirectional").Append(separator); //47
                sb.Append("ISuiteType").Append(separator); //48
                sb.Append("ISuiteNumber").Append(separator); //49
                sb.Append("IPostOfficeBoxType").Append(separator); //50
                sb.Append("IPostOfficeBoxNumber").Append(separator); //51
                sb.Append("ICity").Append(separator); //52
                sb.Append("IConsolidatedCity").Append(separator); //53
                sb.Append("IMinorCivilDivision").Append(separator); //54
                sb.Append("ICountySubRegion").Append(separator); //55
                sb.Append("ICounty").Append(separator); //56
                sb.Append("IState").Append(separator); //57
                sb.Append("IZip").Append(separator); //58
                sb.Append("IZipPlus1").Append(separator); //59
                sb.Append("IZipPlus2").Append(separator); //60
                sb.Append("IZipPlus3").Append(separator); //61
                sb.Append("IZipPlus4").Append(separator); //62
                sb.Append("IZipPlus5").Append(separator); //63
            }

            if (options == null || options.OutputMatchedAddressFieldMappings.Enabled)
            {
                sb.Append("MNumber").Append(separator); //37
                sb.Append("MNumberFractional").Append(separator); //38
                sb.Append("MPreDirectional").Append(separator); //39
                sb.Append("MPreQualifier").Append(separator); //40
                sb.Append("MPreType").Append(separator); //41
                sb.Append("MPreArticle").Append(separator); //42
                sb.Append("MName").Append(separator); //43
                sb.Append("MPostArticle").Append(separator); //44
                sb.Append("MPostQualifier").Append(separator); //45
                sb.Append("MSuffix").Append(separator); //46
                sb.Append("MPostDirectional").Append(separator); //47
                sb.Append("MSuiteType").Append(separator); //48
                sb.Append("MSuiteNumber").Append(separator); //49
                sb.Append("MPostOfficeBoxType").Append(separator); //50
                sb.Append("MPostOfficeBoxNumber").Append(separator); //51
                sb.Append("MCity").Append(separator); //52
                sb.Append("MConsolidatedCity").Append(separator); //53
                sb.Append("MMinorCivilDivision").Append(separator); //54
                sb.Append("MCountySubRegion").Append(separator); //55
                sb.Append("MCounty").Append(separator); //56
                sb.Append("MState").Append(separator); //57
                sb.Append("MZip").Append(separator); //58
                sb.Append("MZipPlus1").Append(separator); //59
                sb.Append("MZipPlus2").Append(separator); //60
                sb.Append("MZipPlus3").Append(separator); //61
                sb.Append("MZipPlus4").Append(separator); //62
                sb.Append("MZipPlus5").Append(separator); //63
            }


            if (options == null || options.OutputParsedAddressFieldMappings.Enabled)
            {
                sb.Append("PNumber").Append(separator); //64
                sb.Append("PNumberFractional").Append(separator); //65
                sb.Append("PPreDirectional").Append(separator); //66
                sb.Append("PPreQualifier").Append(separator); //67
                sb.Append("PPreType").Append(separator); //68
                sb.Append("PPreArticle").Append(separator); //69
                sb.Append("PName").Append(separator); //70
                sb.Append("PPostArticle").Append(separator); //71
                sb.Append("PPostQualifier").Append(separator); //72
                sb.Append("PSuffix").Append(separator); //73
                sb.Append("PPostDirectional").Append(separator); //74
                sb.Append("PSuiteType").Append(separator); //75
                sb.Append("PSuiteNumber").Append(separator); //76
                sb.Append("PPostOfficeBoxType").Append(separator); //77
                sb.Append("PPostOfficeBoxNumber").Append(separator); //78
                sb.Append("PCity").Append(separator); //79
                sb.Append("PConsolidatedCity").Append(separator); //80
                sb.Append("PMinorCivilDivision").Append(separator); //81
                sb.Append("PCountySubRegion").Append(separator); //82
                sb.Append("PCounty").Append(separator); //83
                sb.Append("PState").Append(separator); //84
                sb.Append("PZip").Append(separator); //85
                sb.Append("PZipPlus1").Append(separator); //86
                sb.Append("PZipPlus2").Append(separator); //87
                sb.Append("PZipPlus3").Append(separator); //88
                sb.Append("PZipPlus4").Append(separator); //89
                sb.Append("PZipPlus5").Append(separator); //90
            }

            if (options == null || options.OutputReferenceFeatureFieldMappings.Enabled)
            {
                sb.Append("FNumber").Append(separator); //91
                sb.Append("FNumberFractional").Append(separator); //92
                sb.Append("FPreDirectional").Append(separator); //93
                sb.Append("FPreQualifier").Append(separator); //94
                sb.Append("FPreType").Append(separator); //95
                sb.Append("FPreArticle").Append(separator); //96
                sb.Append("FName").Append(separator); //97
                sb.Append("FPostArticle").Append(separator); //98
                sb.Append("FPostQualifier").Append(separator); //99
                sb.Append("FSuffix").Append(separator); //100
                sb.Append("FPostDirectional").Append(separator); //101
                sb.Append("FSuiteType").Append(separator); //102
                sb.Append("FSuiteNumber").Append(separator); //103
                sb.Append("FPostOfficeBoxType").Append(separator); //104
                sb.Append("FPostOfficeBoxNumber").Append(separator); //105
                sb.Append("FCity").Append(separator); //106
                sb.Append("FConsolidatedCity").Append(separator); //107
                sb.Append("FMinorCivilDivision").Append(separator); //108
                sb.Append("FCountySubRegion").Append(separator); //109
                sb.Append("FCounty").Append(separator); //110
                sb.Append("FState").Append(separator); //111
                sb.Append("FZip").Append(separator); //112
                sb.Append("FZipPlus1").Append(separator); //113
                sb.Append("FZipPlus2").Append(separator); //114
                sb.Append("FZipPlus3").Append(separator); //115
                sb.Append("FZipPlus4").Append(separator); //116
                sb.Append("FZipPlus5").Append(separator); //117
                sb.Append("FArea").Append(separator); //118
                sb.Append("FAreaType").Append(separator); //119
                sb.Append("FGeometrySRID").Append(separator); //120
                sb.Append("FGeometry").Append(separator); //121
                sb.Append("FSource").Append(separator); //122
                sb.Append("FVintage").Append(separator); //123
                sb.Append("FPrimaryIdField").Append(separator); //124
                sb.Append("FPrimaryIdValue").Append(separator); //125
                sb.Append("FSecondaryIdField").Append(separator); //126
                sb.Append("FSecondaryIdValue"); //127
            }

            return sb.ToString();
        }
    }
}
