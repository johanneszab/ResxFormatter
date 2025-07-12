package plugins.resxformatter.rider

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class ResxFormatterOptionsPage : SimpleOptionsPage(
    name = ResxFormatterBundle.message("configurable.name.resxformatter.options.title"),
    pageId = "ResxFormatterOptionsPage"
) {
    override fun getId(): String = pageId
}