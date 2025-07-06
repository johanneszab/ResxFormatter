package model.rider

import com.jetbrains.rd.generator.nova.Ext
import com.jetbrains.rd.generator.nova.PredefinedType.bool
import com.jetbrains.rd.generator.nova.PredefinedType.string
import com.jetbrains.rd.generator.nova.async
import com.jetbrains.rd.generator.nova.call
import com.jetbrains.rd.generator.nova.field
import com.jetbrains.rider.model.nova.ide.SolutionModel

@Suppress("unused")
object ResxFormatterModel : Ext(SolutionModel.Solution) {

    private val RdResxFormatterFormattingRequest = structdef {
        field("filePath", string)
        field("documentText", string)
    }

    private val RdResxFormatterFormattingResult = structdef {
        field("isSuccess", bool)
        field("hasUpdated", bool)
        field("formattedText", string)
    }

    init {
        call("performReformat", RdResxFormatterFormattingRequest, RdResxFormatterFormattingResult).async
    }
}