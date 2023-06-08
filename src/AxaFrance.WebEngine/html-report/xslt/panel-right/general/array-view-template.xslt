<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">

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