<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<!-- Группировка Мюнчиана по name + surname -->
	<xsl:key name="employees" match="item" use="concat(@name,'|',@surname)" />

	<xsl:output method="xml" indent="yes" />

	<xsl:template match="/">
		<Employees>
			<!-- Проходим по всем item и выбираем только "первые" уникальные -->
			<xsl:for-each select="//item[generate-id() = generate-id(key('employees', concat(@name,'|',@surname))[1])]">
				<Employee name="{@name}" surname="{@surname}">
					<!-- Все salary данного сотрудника -->
					<xsl:for-each select="key('employees', concat(@name,'|',@surname))">
						<salary amount="{@amount}" mount="{@mount}" />
					</xsl:for-each>
				</Employee>
			</xsl:for-each>
		</Employees>
	</xsl:template>

</xsl:stylesheet>
