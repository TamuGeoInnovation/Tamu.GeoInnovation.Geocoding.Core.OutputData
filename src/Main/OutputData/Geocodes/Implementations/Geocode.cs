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
    public class Geocode : AbstractGeocode, IGeocode
    {

        #region Properties





        public new string Quality
        {
            get { return QualityUtils.getQualityName(GeocodeQualityType); }
        }



        private double _MatchScore;
        public new double MatchScore
        {
            get
            {
                if (!MatchScoreSet)
                {
                    if (MatchedFeature != null)
                    {
                        _MatchScore = MatchedFeature.MatchScore;
                    }

                    MatchScoreSet = true;
                }

                return _MatchScore;
            }
            set
            {
                _MatchScore = value;
                MatchScoreSet = true;
            }
        }

        public new string MatchedLocationType
        {
            get { return Statistics.MatchedLocationTypeStatistics.MatchedLocationTypeName; }
        }

        public new string MatchType
        {
            get
            {
                string ret = "";

                if (MatchedFeature != null)
                {
                    if (MatchedFeature.Valid)
                    {
                        ret = FeatureMatcher.GetFeatureMatchTypeName(MatchedFeature.FeatureMatchTypes, ";");
                    }
                    else
                    {
                        if (MatchedFeature.IsExactMatch)
                        {
                            ret = FeatureMatcher.FEATURE_MATCH_TYPE_NAME_EXACT;
                        }
                        else
                        {
                            if (MatchedFeature.IsRelaxedMatch)
                            {
                                if (!String.IsNullOrEmpty(ret))
                                {
                                    ret += "; ";
                                }
                                ret += FeatureMatcher.FEATURE_MATCH_TYPE_NAME_RELAXED;
                            }

                            if (MatchedFeature.IsSubstringMatch)
                            {
                                if (!String.IsNullOrEmpty(ret))
                                {
                                    ret += "; ";
                                }
                                ret += FeatureMatcher.FEATURE_MATCH_TYPE_NAME_SUBSTRING;
                            }

                            if (MatchedFeature.IsSoundexMatch)
                            {
                                if (!String.IsNullOrEmpty(ret))
                                {
                                    ret += "; ";
                                }
                                ret += FeatureMatcher.FEATURE_MATCH_TYPE_NAME_SOUNDEX;
                            }

                            if (MatchedFeature.IsCompositeMatch)
                            {
                                if (!String.IsNullOrEmpty(ret))
                                {
                                    ret += "; ";
                                }
                                ret += FeatureMatcher.FEATURE_MATCH_TYPE_NAME_COMPOSITE;
                            }
                        }
                    }
                }
                else
                {
                    ret += FeatureMatcher.FEATURE_MATCH_TYPE_NAME_NOMATCH;
                }

                return ret;
            }
        }

        public new string CoordinateString
        {
            get { return Geometry.CoordinateString; }
        }

        public new double Latitude
        {
            get
            {
                double ret = 0.0;

                if (Geometry != null)
                {
                    if (Geometry is Point)
                    {
                        ret = ((Point)Geometry).Y;
                    }
                    else if (Geometry is Polygon)
                    {
                        ret = ((Polygon)Geometry).Cy;
                    }
                    else
                    {
                        throw new Exception("Unexpected GeometryType: " + Geometry.GetType());
                    }
                }
                return ret;
            }
        }

        public new double Longitude
        {
            get
            {
                double ret = 0.0;
                if (Geometry != null)
                {
                    if (Geometry is Point)
                    {
                        ret = ((Point)Geometry).X;
                    }
                    else if (Geometry is Polygon)
                    {
                        ret = ((Polygon)Geometry).Cx;
                    }
                    else
                    {
                        throw new Exception("Unexpected GeometryType: " + Geometry.GetType());
                    }
                }
                return ret;
            }
        }

        #endregion

        public Geocode()
            : base() { }

        public Geocode(double version)
            : base(version) { }

        public Geocode(Version versionNew)
            : base(versionNew) { }



        public override void SetMatchedLocationType(int matchedLocationType)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationType);
        }

        public override void SetMatchedLocationType(string matchedLocationTypeName)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationTypeName);
        }

        public override void SetMatchedLocationType(MatchedLocationTypes matchedLocationType)
        {
            Statistics.MatchedLocationTypeStatistics.setMatchedLocationType(matchedLocationType);
        }

        public override string ToString(bool verbose)
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

        public override string AsStringVerbose_V03_01(string separator, double version, BatchOptions options)
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
    }
}
