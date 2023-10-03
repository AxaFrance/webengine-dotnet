<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">

    <xsl:import href="tab/tab-test-case-template.xslt"/>
    <xsl:import href="tab/tab-action-template.xslt"/>
    <xsl:import href="content-tab/content-tab-information-template.xslt"/>
    <xsl:import href="content-tab/content-tab-log-template.xslt"/>
    <xsl:import href="content-tab/content-tab-context-value-template.xslt"/>

    <xsl:template name="content-view-template">
        <xsl:for-each select="we:TestResult">
            <xsl:call-template name="content-view-test-case-template-by-id">
                <xsl:with-param name="id" select="we:Id"/>
            </xsl:call-template>
        </xsl:for-each>

        <xsl:for-each select="//we:ActionReport">
            <xsl:call-template name="content-view-action-template-by-id">
                <xsl:with-param name="id" select="we:Id"/>
            </xsl:call-template>
        </xsl:for-each>
    </xsl:template>

    <xsl:template name="content-view-test-case-template-by-id">
        <xsl:param name="id"/>
        <div id="{$id}" class="body-right-container body-right-container-class">
            <div class="content-header-container">
                <xsl:call-template name="tab-test-case-template">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
            </div>
            <div class="content-container">
                <xsl:call-template name="content-view-general-template-by-id">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
                <xsl:call-template name="content-tab-test-data-template">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
            </div>
        </div>
    </xsl:template>

    <xsl:template name="content-view-action-template-by-id">
        <xsl:param name="id"/>
        <div id="{$id}" class="body-right-container body-right-container-class">
            <div class="content-header-container">
                <xsl:call-template name="tab-action-template">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
            </div>
            <div class="content-container">
                <xsl:call-template name="content-view-general-template-by-id">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
                <xsl:call-template name="content-tab-context-value-template">
                    <xsl:with-param name="id" select="$id"></xsl:with-param>
                </xsl:call-template>
            </div>
        </div>
    </xsl:template>

    <xsl:template name="content-view-general-template-by-id">
        <xsl:param name="id"/>
        <xsl:call-template name="content-tab-information-template">
            <xsl:with-param name="id" select="$id"></xsl:with-param>
        </xsl:call-template>
        <xsl:call-template name="content-tab-log-template">
            <xsl:with-param name="id" select="$id"></xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="content-tab-test-data-template">
        <xsl:param name="id"/>
        <div id="content-id-test-data-{$id}" class="tab-content-container class-container-tab" style="display:none">
            <xsl:call-template name="array-view-template">
                <xsl:with-param name="parentTag" select="we:TestData"></xsl:with-param>
            </xsl:call-template>
        </div>
    </xsl:template>

    <xsl:template name="array-view-template">
        <xsl:param name="parentTag"/>
        <table class="table-common">
            <tr>
                <th>Key</th>
                <th>Value</th>
            </tr>
            <xsl:choose>
                <xsl:when test="$parentTag">
                    <xsl:for-each select="$parentTag/we:Variable">
                        <tr>
                            <td>
                                <xsl:value-of select="we:Name"/>
                            </td>
                            <td>
                                <xsl:value-of select="we:Value"/>
                            </td>
                        </tr>
                    </xsl:for-each>
                </xsl:when>
            </xsl:choose>
        </table>
    </xsl:template>

</xsl:stylesheet>