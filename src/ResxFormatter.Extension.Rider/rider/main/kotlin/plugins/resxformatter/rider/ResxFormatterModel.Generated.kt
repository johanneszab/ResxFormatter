@file:Suppress("EXPERIMENTAL_API_USAGE","EXPERIMENTAL_UNSIGNED_LITERALS","PackageDirectoryMismatch","UnusedImport","unused","LocalVariableName","CanBeVal","PropertyName","EnumEntryName","ClassName","ObjectPropertyName","UnnecessaryVariable","SpellCheckingInspection")
package com.jetbrains.rd.ide.model

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.time.Duration
import kotlin.reflect.KClass
import kotlin.jvm.JvmStatic



/**
 * #### Generated from [ResxFormatterModel.kt:12]
 */
class ResxFormatterModel private constructor(
    private val _performReformat: RdCall<RdResxFormatterFormattingRequest, RdResxFormatterFormattingResult>
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers)  {
            val classLoader = javaClass.classLoader
            serializers.register(LazyCompanionMarshaller(RdId(5191407056163204285), classLoader, "com.jetbrains.rd.ide.model.RdResxFormatterFormattingRequest"))
            serializers.register(LazyCompanionMarshaller(RdId(762521004189503535), classLoader, "com.jetbrains.rd.ide.model.RdResxFormatterFormattingResult"))
        }
        
        
        
        
        
        const val serializationHash = -6843957654235836186L
        
    }
    override val serializersOwner: ISerializersOwner get() = ResxFormatterModel
    override val serializationHash: Long get() = ResxFormatterModel.serializationHash
    
    //fields
    val performReformat: IRdCall<RdResxFormatterFormattingRequest, RdResxFormatterFormattingResult> get() = _performReformat
    //methods
    //initializer
    init {
        _performReformat.async = true
    }
    
    init {
        bindableChildren.add("performReformat" to _performReformat)
    }
    
    //secondary constructor
    internal constructor(
    ) : this(
        RdCall<RdResxFormatterFormattingRequest, RdResxFormatterFormattingResult>(RdResxFormatterFormattingRequest, RdResxFormatterFormattingResult)
    )
    
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("ResxFormatterModel (")
        printer.indent {
            print("performReformat = "); _performReformat.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    override fun deepClone(): ResxFormatterModel   {
        return ResxFormatterModel(
            _performReformat.deepClonePolymorphic()
        )
    }
    //contexts
    //threading
    override val extThreading: ExtThreadingKind get() = ExtThreadingKind.Default
}
val Solution.resxFormatterModel get() = getOrCreateExtension("resxFormatterModel", ::ResxFormatterModel)



/**
 * #### Generated from [ResxFormatterModel.kt:14]
 */
data class RdResxFormatterFormattingRequest (
    val filePath: String,
    val documentText: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<RdResxFormatterFormattingRequest> {
        override val _type: KClass<RdResxFormatterFormattingRequest> = RdResxFormatterFormattingRequest::class
        override val id: RdId get() = RdId(5191407056163204285)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): RdResxFormatterFormattingRequest  {
            val filePath = buffer.readString()
            val documentText = buffer.readString()
            return RdResxFormatterFormattingRequest(filePath, documentText)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: RdResxFormatterFormattingRequest)  {
            buffer.writeString(value.filePath)
            buffer.writeString(value.documentText)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as RdResxFormatterFormattingRequest
        
        if (filePath != other.filePath) return false
        if (documentText != other.documentText) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + filePath.hashCode()
        __r = __r*31 + documentText.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("RdResxFormatterFormattingRequest (")
        printer.indent {
            print("filePath = "); filePath.print(printer); println()
            print("documentText = "); documentText.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
    //threading
}


/**
 * #### Generated from [ResxFormatterModel.kt:19]
 */
data class RdResxFormatterFormattingResult (
    val isSuccess: Boolean,
    val hasUpdated: Boolean,
    val formattedText: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<RdResxFormatterFormattingResult> {
        override val _type: KClass<RdResxFormatterFormattingResult> = RdResxFormatterFormattingResult::class
        override val id: RdId get() = RdId(762521004189503535)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): RdResxFormatterFormattingResult  {
            val isSuccess = buffer.readBool()
            val hasUpdated = buffer.readBool()
            val formattedText = buffer.readString()
            return RdResxFormatterFormattingResult(isSuccess, hasUpdated, formattedText)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: RdResxFormatterFormattingResult)  {
            buffer.writeBool(value.isSuccess)
            buffer.writeBool(value.hasUpdated)
            buffer.writeString(value.formattedText)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as RdResxFormatterFormattingResult
        
        if (isSuccess != other.isSuccess) return false
        if (hasUpdated != other.hasUpdated) return false
        if (formattedText != other.formattedText) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + isSuccess.hashCode()
        __r = __r*31 + hasUpdated.hashCode()
        __r = __r*31 + formattedText.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("RdResxFormatterFormattingResult (")
        printer.indent {
            print("isSuccess = "); isSuccess.print(printer); println()
            print("hasUpdated = "); hasUpdated.print(printer); println()
            print("formattedText = "); formattedText.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
    //threading
}
