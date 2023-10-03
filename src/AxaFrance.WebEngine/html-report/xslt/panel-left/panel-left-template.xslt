<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">

    <xsl:import href="status-template.xslt"/>

    <xsl:template name="sub-tree-view-template">
        <xsl:param name="firstNode"/>
        <ul class="nested">
            <xsl:for-each select="$firstNode">
                <li>
                    <xsl:choose>
                        <xsl:when test="we:SubActionReports">
                            <span class="caret"></span>
                            <xsl:call-template name="status-template">
                                <xsl:with-param name="status" select="we:Result"/>
                                <xsl:with-param name="label" select="we:Name"/>
                                <xsl:with-param name="id" select="we:Id"/>
                            </xsl:call-template>

                            <xsl:call-template name="sub-tree-view-template">
                                <xsl:with-param name="firstNode" select="we:SubActionReports/we:ActionReport"/>
                            </xsl:call-template>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:call-template name="status-template">
                                <xsl:with-param name="status" select="we:Result"/>
                                <xsl:with-param name="label" select="we:Name"/>
                                <xsl:with-param name="id" select="we:Id"/>
                            </xsl:call-template>
                        </xsl:otherwise>
                    </xsl:choose>
                </li>
            </xsl:for-each>
        </ul>
    </xsl:template>

    <xsl:template name="tree-view-template">
        <div id="body-left-container" class="body-left-container">
            <div id='id-tree-header-container' class="tree-header-container">
                <h1>Test cases</h1>
            </div>
            <div id='id-tree-container' class="tree-container">
                <ul id="idTree">
                    <xsl:for-each select="we:TestResult">
                        <li>
                            <span class="caret">
                                <xsl:call-template name="status-template">
                                    <xsl:with-param name="status" select="we:Result"/>
                                    <xsl:with-param name="label" select="we:TestName"/>
                                    <xsl:with-param name="id" select="we:Id"/>
                                </xsl:call-template>
                            </span>

                            <xsl:call-template name="sub-tree-view-template">
                                <xsl:with-param name="firstNode" select="we:ActionReports/we:ActionReport"/>
                            </xsl:call-template>
                        </li>
                    </xsl:for-each>
                </ul>
            </div>
        </div>
    </xsl:template>

</xsl:stylesheet>