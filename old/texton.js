var CompositeDisposable = require('atom').CompositeDisposable;
var child_process = require('child_process');
$ = jQuery = require('jquery');
var jqueryUi = require('jquery-ui');
var atomSpaceView = require('atom-space-pen-views');
var fs = require('fs');
var util = require('util');
var path = require('path');
var Emitter = require('event-kit').Emitter;
var GeneratorPane = require('./generator-pane-view');

function wrappedFunScript() { 
var list_1_Tuple_2_String__String__NilTuple_2_String__String_, list_1_Tuple_2_String__String__ConsTuple_2_String__String_, list_1_Tuple_2_IPane__Object__NilTuple_2_IPane__Object_, list_1_Tuple_2_IPane__Object__ConsTuple_2_IPane__Object_, list_1_String____NilString___, list_1_String____ConsString___, list_1_String__NilString, list_1_String__ConsString, list_1_Object____NilObject___, list_1_Object____ConsObject___, list_1_JQuery__NilJQuery_, list_1_JQuery__ConsJQuery_, Web__sendRequest$Unit_Unit_, WebResponse___ctor$, WebResponse__GetResponseStream$, WebRequest__set_Method$, WebRequest__get_Method$, WebRequest__get_Headers$, WebRequest___ctor$, WebRequest__GetRequestStream$, WebRequest__Create$, WebRequest__AsyncGetResponse$, WebHeaderCollection__get_Values$, WebHeaderCollection__get_Keys$, WebHeaderCollection___ctor$, WebHeaderCollection__Add$, UpdateGeneratorRequest___ctor$, UnfoldEnumerator_2_Int32__String_____ctor$Int32_String___, UnfoldEnumerator_2_Int32__Object___ctor$Int32_Object_, UnfoldEnumerator_2_Int32__IPane___ctor$Int32_IPane_, UnfoldEnumerator_2_Int32__Disposable___ctor$Int32_Disposable_, UnfoldEnumerator_2_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_, UnfoldEnumerator_2_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_, UnfoldEnumerator_2_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, UnfoldEnumerator_2_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, UnfoldEnumerator_2_FSharpList_1_String____String_____ctor$FSharpList_1_String____String___, UnfoldEnumerator_2_FSharpList_1_Object____Object_____ctor$FSharpList_1_Object____Object___, UnfoldEnumerator_2_Boolean__Tuple_2_IPane__Object___ctor$Boolean_Tuple_2_IPane__Object_, UTF8Encoding___ctor$, UTF8Encoding__GetString$, UTF8Encoding__GetBytes$, TupleTuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_, TupleTuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_, TupleString____Int32, TupleString____FSharpList_1_String___, TupleString_String, TupleObject____FSharpList_1_Object___, TupleObject__Int32, TupleJQuery__FSharpList_1_JQuery_, TupleIPane__Object_, TupleIPane__Int32, TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_, TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_, TupleFSharpOption_1_Result_1_Error____FSharpOption_1_Result_1_LintWarning___, TupleFSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_, TupleFSharpFunc_2_WebResponse__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_, TupleFSharpFunc_2_Unit__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_, TupleDisposable__Int32, TimeSpan__get_TicksPerMillisecond$, TextOnViewHelpers__stopMovingHandler$Object_Object_, TextOnViewHelpers__jq$, TextOnViewHelpers__isTextOnEditor$, TextOnViewHelpers__clearTimer$Timer_Timer_, TextOnViewHelpers__OnCursorStopMoving$Object_Object_, TextOnProcess__textonPath$, TextOnProcess__spawn$, TextOnProcess__isWin$, TextOnProcess__getCwd$, TextOnProcess__fromPath$, TextOnIDE__provideErrors$, TextOnIDE__deactivate$, TextOnIDE__activate$, TextOnIDE___ctor$, TextOnGenerator__updateGenerator$, TextOnGenerator__deactivate$, TextOnGenerator__activate$, TextOnGenerator___ctor$, TextOnCommands__openSettings$, String__StartsWith$, String__SplitWithoutOptions$, String__Replace$, String__PrintFormatToString$, String__EndsWith$, Stream__get_Contents$, Stream___ctor$, Stream__Write$, Seq__Unfold$Int32__String___Int32_String___, Seq__Unfold$Int32__Object_Int32_Object_, Seq__Unfold$Int32__IPane_Int32_IPane_, Seq__Unfold$Int32__Disposable_Int32_Disposable_, Seq__Unfold$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_, Seq__Unfold$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_, Seq__Unfold$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, Seq__Unfold$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, Seq__Unfold$FSharpList_1_String____String___FSharpList_1_String____String___, Seq__Unfold$FSharpList_1_Object____Object___FSharpList_1_Object____Object___, Seq__Unfold$Boolean__Tuple_2_IPane__Object_Boolean_Tuple_2_IPane__Object_, Seq__ToList$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__ToArray$String___String___, Seq__ToArray$Object___Object___, Seq__Singleton$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__OfList$String___String___, Seq__OfList$Object___Object___, Seq__OfArray$String___String___, Seq__OfArray$Object_Object_, Seq__OfArray$IPane_IPane_, Seq__Map$Object__IEnumerable_1_Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object_, Seq__Map$IPane__IEnumerable_1_Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object_, Seq__IterateIndexed$String___String___, Seq__IterateIndexed$Object___Object___, Seq__Iterate$Disposable_Disposable_, Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__FromFactory$String___String___, Seq__FromFactory$Object___Object___, Seq__FromFactory$Object_Object_, Seq__FromFactory$IPane_IPane_, Seq__FromFactory$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_, Seq__FromFactory$Disposable_Disposable_, Seq__FoldIndexedAux$Unit__String___Unit__String___, Seq__FoldIndexedAux$Unit__Object___Unit__Object___, Seq__FoldIndexedAux$Unit__Disposable_Unit__Disposable_, Seq__FoldIndexedAux$FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, Seq__FoldIndexed$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_, Seq__FoldIndexed$String____Unit_String____Unit_, Seq__FoldIndexed$Object____Unit_Object____Unit_, Seq__FoldIndexed$Disposable__Unit_Disposable__Unit_, Seq__Fold$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_, Seq__Fold$Disposable__Unit_Disposable__Unit_, Seq__Enumerator$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__Enumerator$String___String___, Seq__Enumerator$Object___Object___, Seq__Enumerator$Object_Object_, Seq__Enumerator$IPane_IPane_, Seq__Enumerator$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_, Seq__Enumerator$Disposable_Disposable_, Seq__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__Delay$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Seq__Delay$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_, Seq__Concat$IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, Seq__Collect$Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, Seq__Collect$IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, ResizeArray__ToSeq$Disposable_Disposable_, ResizeArray_1_Object__get_Item$Object_, ResizeArray_1_Object__get_Count$Object_, Replacements__utf8Encoding$, Provider___ctor$, ParseRequest___ctor$, Options___ctor$, Option__Iterate$Timer_Timer_, Option__Iterate$String_1String, Option__Iterate$IEditor_IEditor_, Option__Iterate$Disposable_Disposable_, Option__Iterate$ChildProcess_ChildProcess_, Option__IsSome$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Option__IsSome$Result_1_NavigateData_Result_1_NavigateData_, Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_, Option__IsSome$Int32_Int32, Option__IsSome$IEnumerator_1_Object_IEnumerator_1_Object_, Option__IsSome$IEnumerator_1_IPane_IEnumerator_1_IPane_, Option__IsSome$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_, Option__IsSome$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_, Option__IsSome$FSharpList_1_String___FSharpList_1_String___, Option__IsSome$FSharpList_1_Object___FSharpList_1_Object___, Option__IsSome$Boolean_Boolean, Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_, Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_, Option__GetValue$Tuple_2_Tuple_2_IPane__Object__Boolean_Tuple_2_Tuple_2_IPane__Object__Boolean_, Option__GetValue$Tuple_2_String____Int32_Tuple_2_String____Int32_, Option__GetValue$Tuple_2_String____FSharpList_1_String___Tuple_2_String____FSharpList_1_String___, Option__GetValue$Tuple_2_Object____FSharpList_1_Object___Tuple_2_Object____FSharpList_1_Object___, Option__GetValue$Tuple_2_Object__Int32_Tuple_2_Object__Int32_, Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, Option__GetValue$Tuple_2_IPane__Int32_Tuple_2_IPane__Int32_, Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_, Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_, Option__GetValue$Tuple_2_Disposable__Int32_Tuple_2_Disposable__Int32_, Option__GetValue$Timer_Timer_, Option__GetValue$String_1String, Option__GetValue$Result_1_Unit_Result_1_Unit_, Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_, Option__GetValue$Result_1_LintWarning___Result_1_LintWarning___, Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_, Option__GetValue$Result_1_Error___Result_1_Error___, Option__GetValue$Int32_Int32, Option__GetValue$IEnumerator_1_Tuple_2_IPane__Object_IEnumerator_1_Tuple_2_IPane__Object_, Option__GetValue$IEnumerator_1_Object_IEnumerator_1_Object_, Option__GetValue$IEnumerator_1_IPane_IEnumerator_1_IPane_, Option__GetValue$IEditor_IEditor_, Option__GetValue$FSharpRef_1_Boolean_FSharpRef_1_Boolean_, Option__GetValue$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_, Option__GetValue$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_, Option__GetValue$FSharpList_1_String___FSharpList_1_String___, Option__GetValue$FSharpList_1_Object___FSharpList_1_Object___, Option__GetValue$Disposable_Disposable_, Option__GetValue$ChildProcess_ChildProcess_, Option__GetValue$CancellationToken_CancellationToken_, Option__GetValue$Boolean_Boolean, OpenOptions___ctor$, OpenEditorOptions___ctor$, NavigateRequest___ctor$, Logger__subscriptions, Logger__logf$, Logger__logPath, Logger__get_subscriptions$, Logger__get_logPath$, Logger__get_fullLog$, Logger__get_editor$, Logger__get_active$, Logger__fullLog, Logger__emitLog$, Logger__editor, Logger__deactivate$, Logger__active, Logger__activate$, List__TryPickIndexedAux$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, List__TryPickIndexed$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, List__TryPick$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, List__ToArray$String_1String, List__Tail$String___String___, List__Tail$Object___Object___, List__Reverse$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, List__Reverse$String_1String, List__Reverse$JQuery_JQuery_, List__OfArray$String_1String, List__Map$Tuple_2_String__String__String_1Tuple_2_String__String__String, List__Length$String_1String, List__IterateIndexed$String_1String, List__Head$String___String___, List__Head$Object___Object___, List__FoldIndexedAux$list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_, List__FoldIndexedAux$list_1_String__Tuple_2_String__String_list_1_String__Tuple_2_String__String_, List__FoldIndexedAux$list_1_String__String_1list_1_String__String, List__FoldIndexedAux$list_1_JQuery__JQuery_list_1_JQuery__JQuery_, List__FoldIndexedAux$Unit__String_1Unit__String, List__FoldIndexedAux$JQuery__String_1JQuery__String, List__FoldIndexedAux$JQuery__JQuery_JQuery__JQuery_, List__FoldIndexedAux$Int32__String_1Int32_String, List__FoldIndexed$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_, List__FoldIndexed$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_, List__FoldIndexed$String_1_list_1_String_String_list_1_String_, List__FoldIndexed$String_1_Unit_String_Unit_, List__FoldIndexed$String_1_JQuery_String_JQuery_, List__FoldIndexed$String_1_Int32_String_Int32, List__FoldIndexed$JQuery__list_1_JQuery_JQuery__list_1_JQuery_, List__FoldIndexed$JQuery__JQuery_JQuery__JQuery_, List__Fold$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_, List__Fold$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_, List__Fold$String_1_list_1_String_String_list_1_String_, List__Fold$String_1_JQuery_String_JQuery_, List__Fold$String_1_Int32_String_Int32, List__Fold$JQuery__list_1_JQuery_JQuery__list_1_JQuery_, List__Fold$JQuery__JQuery_JQuery__JQuery_, List__Empty$Tuple_2_String__String_Tuple_2_String__String_, List__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, List__Empty$String___String___, List__Empty$Object___Object___, List__Empty$JQuery_JQuery_, List__CreateCons$Tuple_2_String__String_Tuple_2_String__String_, List__CreateCons$Tuple_2_IPane__Object_Tuple_2_IPane__Object_, List__CreateCons$String___String___, List__CreateCons$Object___Object___, List__CreateCons$JQuery_JQuery_, LintResult___ctor$, LanguageService__url$, LanguageService__updateGenerator$, LanguageService__tryParse$Result_1_Unit_Result_1_Unit_, LanguageService__tryParse$Result_1_NavigateData_Result_1_NavigateData_, LanguageService__tryParse$Result_1_GeneratorData_Result_1_GeneratorData_, LanguageService__tryParse$Result_1_Error___Result_1_Error___, LanguageService__stop$, LanguageService__start$, LanguageService__service, LanguageService__send$Unit_Unit_, LanguageService__send$NavigateData_NavigateData_, LanguageService__send$GeneratorData_GeneratorData_, LanguageService__send$Error___Error___, LanguageService__request$UpdateGeneratorRequest_UpdateGeneratorRequest_, LanguageService__request$ParseRequest_ParseRequest_, LanguageService__request$NavigateRequest_NavigateRequest_, LanguageService__request$GeneratorValueSetRequest_GeneratorValueSetRequest_, LanguageService__request$GeneratorStopRequest_GeneratorStopRequest_, LanguageService__request$GeneratorStartRequest_GeneratorStartRequest_, LanguageService__request$GenerateRequest_GenerateRequest_, LanguageService__port, LanguageService__parseResponse$Unit_Unit_, LanguageService__parseResponse$NavigateData_NavigateData_, LanguageService__parseResponse$GeneratorData_GeneratorData_, LanguageService__parseResponse$Error___Error___, LanguageService__parseEditor$, LanguageService__parse$, LanguageService__navigateToVariable$, LanguageService__navigateToFunction$, LanguageService__navigateToAttribute$, LanguageService__get_service$, LanguageService__get_port$, LanguageService__generatorValueSet$, LanguageService__generatorStop$, LanguageService__generatorStart$, LanguageService__generate$, LanguageService__genPort$, GeneratorValueSetRequest___ctor$, GeneratorStopRequest___ctor$, GeneratorStartRequest___ctor$, GeneratorPane__valueSet$, GeneratorPane__tryFindTextOnGeneratorPane$, GeneratorPane__th$, GeneratorPane__td$, GeneratorPane__sendToTextOnGenerator$, GeneratorPane__sendToGenerator$, GeneratorPane__replaceTextOnGeneratorHtmlPanel$, GeneratorPane__performGeneration$, GeneratorPane__openTextOnGeneratorPane$, GeneratorPane__navigateToVariable$, GeneratorPane__navigateToFunction$, GeneratorPane__navigateToFileLine$, GeneratorPane__navigateToEditor$, GeneratorPane__navigateToAttribute$, GeneratorPane__makeVariables$, GeneratorPane__makeTitle$, GeneratorPane__makeLinkForVariable$, GeneratorPane__makeLinkForLine$, GeneratorPane__makeLinkForAttribute$, GeneratorPane__makeCombobox$String_1String, GeneratorPane__makeAttributes$, GeneratorPane__getTextOnCursorLine$, GeneratorPane__divPadded$, GeneratorPane__divInsetPadded$, GeneratorPane__divContent$, GeneratorPane__copyToClipboard$, GeneratorConfiguration___ctor$, GenerateRequest___ctor$, ErrorLinterProvider__mapLint$, ErrorLinterProvider__mapError$, ErrorLinterProvider__lint$, ErrorLinterProvider__create$, DateTime__get_Ticks$, DateTime__get_Now$, DateTime__createUnsafe$, DateTime__ToLongTimeString$, CreateEnumerable_1_Tuple_2_IPane__Object___ctor$Tuple_2_IPane__Object_, CreateEnumerable_1_String_____ctor$String___, CreateEnumerable_1_Object___ctor$Object_, CreateEnumerable_1_Object_____ctor$Object___, CreateEnumerable_1_IPane___ctor$IPane_, CreateEnumerable_1_IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerable_1_Tuple_2_IPane__Object_, CreateEnumerable_1_Disposable___ctor$Disposable_, Control__IConfig_onDidChange$ConfigChange_1_Boolean_ConfigChange_1_Boolean_, Control__CommandRegistry_subscribe$, Control__Async_StartAsPromise_Static$Object_Object_, CancellationToken___ctor$, CancellationToken__ThrowIfCancellationRequested$, Async__protectedCont$WebResponse_WebResponse_, Async__protectedCont$Unit_Unit_, Async__protectedCont$String___String___, Async__protectedCont$Object___Object___, Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, Async__invokeCont$Unit_Unit_, Async__invokeCont$String___String___, Async__invokeCont$Object___Object___, Async__invokeCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, Async__invokeCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, Async__invokeCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, Async__invokeCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, Async__get_async$, Async_1_WebResponse__ContWebResponse_, Async_1_Unit__ContUnit_, Async_1_String____ContString___, Async_1_StartImmediate$, Async_1_Object____ContObject___, Async_1_FromContinuations$WebResponse_WebResponse_, Async_1_FromContinuations$Unit_Unit_, Async_1_FSharpOption_1_Result_1_Unit__ContFSharpOption_1_Result_1_Unit_, Async_1_FSharpOption_1_Result_1_NavigateData__ContFSharpOption_1_Result_1_NavigateData_, Async_1_FSharpOption_1_Result_1_GeneratorData__ContFSharpOption_1_Result_1_GeneratorData_, Async_1_FSharpOption_1_Result_1_Error____ContFSharpOption_1_Result_1_Error___, AsyncParams_1_WebResponse___ctor$WebResponse_, AsyncParams_1_Unit___ctor$Unit_, AsyncParams_1_String_____ctor$String___, AsyncParams_1_Object_____ctor$Object___, AsyncParams_1_FSharpOption_1_Result_1_Unit___ctor$FSharpOption_1_Result_1_Unit_, AsyncParams_1_FSharpOption_1_Result_1_NavigateData___ctor$FSharpOption_1_Result_1_NavigateData_, AsyncParams_1_FSharpOption_1_Result_1_GeneratorData___ctor$FSharpOption_1_Result_1_GeneratorData_, AsyncParams_1_FSharpOption_1_Result_1_Error_____ctor$FSharpOption_1_Result_1_Error___, AsyncParamsAux___ctor$, AsyncBuilder___ctor$, AsyncBuilder__Zero$, AsyncBuilder__TryWith$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, AsyncBuilder__TryWith$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, AsyncBuilder__TryWith$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, AsyncBuilder__TryWith$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, AsyncBuilder__Return$Unit_Unit_, AsyncBuilder__Return$String___String___, AsyncBuilder__Return$Object___Object___, AsyncBuilder__Return$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, AsyncBuilder__Return$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, AsyncBuilder__Return$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, AsyncBuilder__Return$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, AsyncBuilder__Delay$Unit_Unit_, AsyncBuilder__Delay$String___String___, AsyncBuilder__Delay$Object___Object___, AsyncBuilder__Delay$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, AsyncBuilder__Delay$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, AsyncBuilder__Delay$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, AsyncBuilder__Delay$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, AsyncBuilder__Combine$Unit_Unit_, AsyncBuilder__Bind$WebResponse__String___WebResponse__String___, AsyncBuilder__Bind$Unit__Unit_Unit__Unit_, AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Unit_String____FSharpOption_1_Result_1_Unit_, AsyncBuilder__Bind$String____FSharpOption_1_Result_1_NavigateData_String____FSharpOption_1_Result_1_NavigateData_, AsyncBuilder__Bind$String____FSharpOption_1_Result_1_GeneratorData_String____FSharpOption_1_Result_1_GeneratorData_, AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Error___String____FSharpOption_1_Result_1_Error___, AsyncBuilder__Bind$Object____Unit_Object____Unit_, AsyncBuilder__Bind$FSharpOption_1_Result_1_NavigateData__Unit_FSharpOption_1_Result_1_NavigateData__Unit_, AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_, AsyncBuilder__Bind$FSharpOption_1_Result_1_Error____Object___FSharpOption_1_Result_1_Error____Object___, Array__ZeroCreate$String___String___, Array__ZeroCreate$String_1String, Array__ZeroCreate$Object___Object___, Array__ZeroCreate$Object_Object_, Array__ZeroCreate$JQuery_JQuery_, Array__ZeroCreate$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_, Array__ZeroCreate$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_, Array__ZeroCreate$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_, Array__ZeroCreate$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___, Array__MapIndexed$String_1_String___String_String___, Array__MapIndexed$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_, Array__MapIndexed$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_, Array__MapIndexed$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_, Array__MapIndexed$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___, Array__MapIndexed$OutputString__String_1OutputString__String, Array__MapIndexed$LintWarning__Object_LintWarning__Object_, Array__MapIndexed$GeneratorVariable__JQuery_GeneratorVariable__JQuery_, Array__MapIndexed$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_, Array__MapIndexed$Error__Object_Error__Object_, Array__Map$String_1_String___String_String___, Array__Map$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_, Array__Map$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_, Array__Map$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_, Array__Map$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___, Array__Map$OutputString__String_1OutputString__String, Array__Map$LintWarning__Object_LintWarning__Object_, Array__Map$GeneratorVariable__JQuery_GeneratorVariable__JQuery_, Array__Map$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_, Array__Map$Error__Object_Error__Object_, Array__Length$String_1String, Array__Length$OutputString_OutputString_, Array__Length$Object_Object_, Array__Length$LintWarning_LintWarning_, Array__Length$JQuery_JQuery_, Array__Length$IPane_IPane_, Array__Length$GeneratorVariable_GeneratorVariable_, Array__Length$GeneratorAttribute_GeneratorAttribute_, Array__Length$Error_Error_, Array__GetSubArray$Byte_Byte, Array__FoldIndexed$Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_, Array__FoldIndexed$String____String_1String____String, Array__FoldIndexed$String_1_String_1String_String, Array__FoldIndexed$JQuery__JQuery_JQuery__JQuery_, Array__FoldBackIndexed$String_1_list_1_String_String_list_1_String_, Array__FoldBack$String_1_list_1_String_String_list_1_String_, Array__Fold$String_1_String___String_String___, Array__Fold$String_1_String_1String_String, Array__Fold$OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_, Array__Fold$JQuery__JQuery_JQuery__JQuery_, Array__ConcatImpl$String_1String, Array__ConcatImpl$Object_Object_, Array__Concat$String_1String, Array__Concat$Object_Object_, Array__BoxedLength$, Array__Append$Object_Object_, Array__Append$Byte_Byte;
Array__Append$Byte_Byte = (function(xs,ys)
{
    return xs.concat(ys);;
});
Array__Append$Object_Object_ = (function(xs,ys)
{
    return xs.concat(ys);;
});
Array__BoxedLength$ = (function(xs)
{
    return xs.length;;
});
Array__Concat$Object_Object_ = (function(xs)
{
    return Array__ConcatImpl$Object_Object_(Seq__ToArray$Object___Object___(xs));
});
Array__Concat$String_1String = (function(xs)
{
    return Array__ConcatImpl$String_1String(Seq__ToArray$String___String___(xs));
});
Array__ConcatImpl$Object_Object_ = (function(xss)
{
    return [].concat.apply([], xss);;
});
Array__ConcatImpl$String_1String = (function(xss)
{
    return [].concat.apply([], xss);;
});
Array__Fold$JQuery__JQuery_JQuery__JQuery_ = (function(f,seed,xs)
{
    return Array__FoldIndexed$JQuery__JQuery_JQuery__JQuery_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Array__Fold$OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_ = (function(f,seed,xs)
{
    return Array__FoldIndexed$Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Array__Fold$String_1_String_1String_String = (function(f,seed,xs)
{
    return Array__FoldIndexed$String_1_String_1String_String((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Array__Fold$String_1_String___String_String___ = (function(f,seed,xs)
{
    return Array__FoldIndexed$String____String_1String____String((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Array__FoldBack$String_1_list_1_String_String_list_1_String_ = (function(f,xs,seed)
{
    return Array__FoldBackIndexed$String_1_list_1_String_String_list_1_String_((function(_arg1)
    {
      return (function(x)
      {
        return (function(acc)
        {
          return f(x)(acc);
        });
      });
    }), xs, seed);
});
Array__FoldBackIndexed$String_1_list_1_String_String_list_1_String_ = (function(f,xs,seed)
{
    var acc = seed;
    var size = Array__Length$String_1String(xs);
    for (var i = 1; i <= size; i++)
    {
      acc = f((i - 1))(xs[(size - i)])(acc);
      null;
    };
    return acc;
});
Array__FoldIndexed$JQuery__JQuery_JQuery__JQuery_ = (function(f,seed,xs)
{
    var acc = seed;
    for (var i = 0; i <= (Array__Length$JQuery_JQuery_(xs) - 1); i++)
    {
      acc = f(i)(acc)(xs[i]);
      null;
    };
    return acc;
});
Array__FoldIndexed$String_1_String_1String_String = (function(f,seed,xs)
{
    var acc = seed;
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      acc = f(i)(acc)(xs[i]);
      null;
    };
    return acc;
});
Array__FoldIndexed$String____String_1String____String = (function(f,seed,xs)
{
    var acc = seed;
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      acc = f(i)(acc)(xs[i]);
      null;
    };
    return acc;
});
Array__FoldIndexed$Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_Tuple_2_JQuery__FSharpList_1_JQuery__OutputString_ = (function(f,seed,xs)
{
    var acc = seed;
    for (var i = 0; i <= (Array__Length$OutputString_OutputString_(xs) - 1); i++)
    {
      acc = f(i)(acc)(xs[i]);
      null;
    };
    return acc;
});
Array__GetSubArray$Byte_Byte = (function(xs,offset,length)
{
    return xs.slice(offset, offset + length);;
});
Array__Length$Error_Error_ = (function(xs)
{
    return xs.length;;
});
Array__Length$GeneratorAttribute_GeneratorAttribute_ = (function(xs)
{
    return xs.length;;
});
Array__Length$GeneratorVariable_GeneratorVariable_ = (function(xs)
{
    return xs.length;;
});
Array__Length$IPane_IPane_ = (function(xs)
{
    return xs.length;;
});
Array__Length$JQuery_JQuery_ = (function(xs)
{
    return xs.length;;
});
Array__Length$LintWarning_LintWarning_ = (function(xs)
{
    return xs.length;;
});
Array__Length$Object_Object_ = (function(xs)
{
    return xs.length;;
});
Array__Length$OutputString_OutputString_ = (function(xs)
{
    return xs.length;;
});
Array__Length$String_1String = (function(xs)
{
    return xs.length;;
});
Array__Map$Error__Object_Error__Object_ = (function(f,xs)
{
    return Array__MapIndexed$Error__Object_Error__Object_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_ = (function(f,xs)
{
    return Array__MapIndexed$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$GeneratorVariable__JQuery_GeneratorVariable__JQuery_ = (function(f,xs)
{
    return Array__MapIndexed$GeneratorVariable__JQuery_GeneratorVariable__JQuery_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$LintWarning__Object_LintWarning__Object_ = (function(f,xs)
{
    return Array__MapIndexed$LintWarning__Object_LintWarning__Object_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$OutputString__String_1OutputString__String = (function(f,xs)
{
    return Array__MapIndexed$OutputString__String_1OutputString__String((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___ = (function(f,xs)
{
    return Array__MapIndexed$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_ = (function(f,xs)
{
    return Array__MapIndexed$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_ = (function(f,xs)
{
    return Array__MapIndexed$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_ = (function(f,xs)
{
    return Array__MapIndexed$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__Map$String_1_String___String_String___ = (function(f,xs)
{
    return Array__MapIndexed$String_1_String___String_String___((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
Array__MapIndexed$Error__Object_Error__Object_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$Object_Object_(Array__Length$Error_Error_(xs));
    for (var i = 0; i <= (Array__Length$Error_Error_(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$JQuery_JQuery_(Array__Length$GeneratorAttribute_GeneratorAttribute_(xs));
    for (var i = 0; i <= (Array__Length$GeneratorAttribute_GeneratorAttribute_(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$GeneratorVariable__JQuery_GeneratorVariable__JQuery_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$JQuery_JQuery_(Array__Length$GeneratorVariable_GeneratorVariable_(xs));
    for (var i = 0; i <= (Array__Length$GeneratorVariable_GeneratorVariable_(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$LintWarning__Object_LintWarning__Object_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$Object_Object_(Array__Length$LintWarning_LintWarning_(xs));
    for (var i = 0; i <= (Array__Length$LintWarning_LintWarning_(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$OutputString__String_1OutputString__String = (function(f,xs)
{
    var ys = Array__ZeroCreate$String_1String(Array__Length$OutputString_OutputString_(xs));
    for (var i = 0; i <= (Array__Length$OutputString_OutputString_(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___ = (function(f,xs)
{
    var ys = Array__ZeroCreate$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Array__Length$String_1String(xs));
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Array__Length$String_1String(xs));
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Array__Length$String_1String(xs));
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_ = (function(f,xs)
{
    var ys = Array__ZeroCreate$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Array__Length$String_1String(xs));
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__MapIndexed$String_1_String___String_String___ = (function(f,xs)
{
    var ys = Array__ZeroCreate$String___String___(Array__Length$String_1String(xs));
    for (var i = 0; i <= (Array__Length$String_1String(xs) - 1); i++)
    {
      ys[i] = f(i)(xs[i]);
      null;
    };
    return ys;
});
Array__ZeroCreate$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$JQuery_JQuery_ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$Object_Object_ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$Object___Object___ = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$String_1String = (function(size)
{
    return new Array(size);;
});
Array__ZeroCreate$String___String___ = (function(size)
{
    return new Array(size);;
});
AsyncBuilder__Bind$FSharpOption_1_Result_1_Error____Object___FSharpOption_1_Result_1_Error____Object___ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$Object___Object___((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_FSharpOption_1_Result_1_Error_____ctor$FSharpOption_1_Result_1_Error___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_FSharpOption_1_Result_1_GeneratorData___ctor$FSharpOption_1_Result_1_GeneratorData_(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$FSharpOption_1_Result_1_NavigateData__Unit_FSharpOption_1_Result_1_NavigateData__Unit_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_FSharpOption_1_Result_1_NavigateData___ctor$FSharpOption_1_Result_1_NavigateData_(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$Object____Unit_Object____Unit_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_Object_____ctor$Object___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Error___String____FSharpOption_1_Result_1_Error___ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_String_____ctor$String___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$String____FSharpOption_1_Result_1_GeneratorData_String____FSharpOption_1_Result_1_GeneratorData_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_String_____ctor$String___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$String____FSharpOption_1_Result_1_NavigateData_String____FSharpOption_1_Result_1_NavigateData_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_String_____ctor$String___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Unit_String____FSharpOption_1_Result_1_Unit_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_String_____ctor$String___(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$Unit__Unit_Unit__Unit_ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_Unit___ctor$Unit_(cont, k.Aux)));
    }));
});
AsyncBuilder__Bind$WebResponse__String___WebResponse__String___ = (function(x,_arg1,f)
{
    var v = _arg1.Item;
    return Async__protectedCont$String___String___((function(k)
    {
      var cont = (function(a)
      {
        var patternInput = f(a);
        var r = patternInput.Item;
        return r(k);
      });
      return v((new AsyncParams_1_WebResponse___ctor$WebResponse_(cont, k.Aux)));
    }));
});
AsyncBuilder__Combine$Unit_Unit_ = (function(x,work1,work2)
{
    return AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(x, work1, (function(unitVar0)
    {
      return work2;
    }));
});
AsyncBuilder__Delay$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(x,f)
{
    return Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___((function(k)
    {
      var _5652;
      var patternInput = f(_5652);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(x,f)
{
    return Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_((function(k)
    {
      var _3225;
      var patternInput = f(_3225);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(x,f)
{
    return Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_((function(k)
    {
      var _3707;
      var patternInput = f(_3707);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(x,f)
{
    return Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_((function(k)
    {
      var _1796;
      var patternInput = f(_1796);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$Object___Object___ = (function(x,f)
{
    return Async__protectedCont$Object___Object___((function(k)
    {
      var _6236;
      var patternInput = f(_6236);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$String___String___ = (function(x,f)
{
    return Async__protectedCont$String___String___((function(k)
    {
      var _1528;
      var patternInput = f(_1528);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Delay$Unit_Unit_ = (function(x,f)
{
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var _3877;
      var patternInput = f(_3877);
      var r = patternInput.Item;
      return r(k);
    }));
});
AsyncBuilder__Return$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(x,v)
{
    return Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___((function(k)
    {
      return Async__invokeCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(k, v);
    }));
});
AsyncBuilder__Return$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(x,v)
{
    return Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_((function(k)
    {
      return Async__invokeCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(k, v);
    }));
});
AsyncBuilder__Return$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(x,v)
{
    return Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_((function(k)
    {
      return Async__invokeCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(k, v);
    }));
});
AsyncBuilder__Return$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(x,v)
{
    return Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_((function(k)
    {
      return Async__invokeCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(k, v);
    }));
});
AsyncBuilder__Return$Object___Object___ = (function(x,v)
{
    return Async__protectedCont$Object___Object___((function(k)
    {
      return Async__invokeCont$Object___Object___(k, v);
    }));
});
AsyncBuilder__Return$String___String___ = (function(x,v)
{
    return Async__protectedCont$String___String___((function(k)
    {
      return Async__invokeCont$String___String___(k, v);
    }));
});
AsyncBuilder__Return$Unit_Unit_ = (function(x,v)
{
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      return Async__invokeCont$Unit_Unit_(k, v);
    }));
});
AsyncBuilder__TryWith$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(x,_arg2,catchFunction)
{
    var v = _arg2.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___((function(k)
    {
      CancellationToken__ThrowIfCancellationRequested$(k.Aux.CancellationToken);
      var inputRecord = k.Aux;
      var ExceptionCont = (function(ex)
      {
        return k.Cont(catchFunction(ex));
      });
      var Aux = (new AsyncParamsAux___ctor$(inputRecord.StackCounter, ExceptionCont, inputRecord.CancelledCont, inputRecord.CancellationToken));
      return v((new AsyncParams_1_FSharpOption_1_Result_1_Error_____ctor$FSharpOption_1_Result_1_Error___(k.Cont, Aux)));
    }));
});
AsyncBuilder__TryWith$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(x,_arg2,catchFunction)
{
    var v = _arg2.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_((function(k)
    {
      CancellationToken__ThrowIfCancellationRequested$(k.Aux.CancellationToken);
      var inputRecord = k.Aux;
      var ExceptionCont = (function(ex)
      {
        return k.Cont(catchFunction(ex));
      });
      var Aux = (new AsyncParamsAux___ctor$(inputRecord.StackCounter, ExceptionCont, inputRecord.CancelledCont, inputRecord.CancellationToken));
      return v((new AsyncParams_1_FSharpOption_1_Result_1_GeneratorData___ctor$FSharpOption_1_Result_1_GeneratorData_(k.Cont, Aux)));
    }));
});
AsyncBuilder__TryWith$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(x,_arg2,catchFunction)
{
    var v = _arg2.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_((function(k)
    {
      CancellationToken__ThrowIfCancellationRequested$(k.Aux.CancellationToken);
      var inputRecord = k.Aux;
      var ExceptionCont = (function(ex)
      {
        return k.Cont(catchFunction(ex));
      });
      var Aux = (new AsyncParamsAux___ctor$(inputRecord.StackCounter, ExceptionCont, inputRecord.CancelledCont, inputRecord.CancellationToken));
      return v((new AsyncParams_1_FSharpOption_1_Result_1_NavigateData___ctor$FSharpOption_1_Result_1_NavigateData_(k.Cont, Aux)));
    }));
});
AsyncBuilder__TryWith$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(x,_arg2,catchFunction)
{
    var v = _arg2.Item;
    return Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_((function(k)
    {
      CancellationToken__ThrowIfCancellationRequested$(k.Aux.CancellationToken);
      var inputRecord = k.Aux;
      var ExceptionCont = (function(ex)
      {
        return k.Cont(catchFunction(ex));
      });
      var Aux = (new AsyncParamsAux___ctor$(inputRecord.StackCounter, ExceptionCont, inputRecord.CancelledCont, inputRecord.CancellationToken));
      return v((new AsyncParams_1_FSharpOption_1_Result_1_Unit___ctor$FSharpOption_1_Result_1_Unit_(k.Cont, Aux)));
    }));
});
AsyncBuilder__Zero$ = (function(x,unitVar1)
{
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      var _3854;
      return Async__invokeCont$Unit_Unit_(k, _3854);
    }));
});
AsyncBuilder___ctor$ = (function(unitVar0)
{
    {};
});
AsyncParamsAux___ctor$ = (function(StackCounter,ExceptionCont,CancelledCont,CancellationToken)
{
    var __this = this;
    __this.StackCounter = StackCounter;
    __this.ExceptionCont = ExceptionCont;
    __this.CancelledCont = CancelledCont;
    __this.CancellationToken = CancellationToken;
});
AsyncParams_1_FSharpOption_1_Result_1_Error_____ctor$FSharpOption_1_Result_1_Error___ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_FSharpOption_1_Result_1_GeneratorData___ctor$FSharpOption_1_Result_1_GeneratorData_ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_FSharpOption_1_Result_1_NavigateData___ctor$FSharpOption_1_Result_1_NavigateData_ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_FSharpOption_1_Result_1_Unit___ctor$FSharpOption_1_Result_1_Unit_ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_Object_____ctor$Object___ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_String_____ctor$String___ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_Unit___ctor$Unit_ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
AsyncParams_1_WebResponse___ctor$WebResponse_ = (function(Cont,Aux)
{
    var __this = this;
    __this.Cont = Cont;
    __this.Aux = Aux;
});
Async_1_FSharpOption_1_Result_1_Error____ContFSharpOption_1_Result_1_Error___ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_FSharpOption_1_Result_1_GeneratorData__ContFSharpOption_1_Result_1_GeneratorData_ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_FSharpOption_1_Result_1_NavigateData__ContFSharpOption_1_Result_1_NavigateData_ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_FSharpOption_1_Result_1_Unit__ContFSharpOption_1_Result_1_Unit_ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_FromContinuations$Unit_Unit_ = (function(f)
{
    return Async__protectedCont$Unit_Unit_((function(k)
    {
      return f((new TupleFSharpFunc_2_Unit__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_(k.Cont, k.Aux.ExceptionCont, k.Aux.CancelledCont)));
    }));
});
Async_1_FromContinuations$WebResponse_WebResponse_ = (function(f)
{
    return Async__protectedCont$WebResponse_WebResponse_((function(k)
    {
      return f((new TupleFSharpFunc_2_WebResponse__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_(k.Cont, k.Aux.ExceptionCont, k.Aux.CancelledCont)));
    }));
});
Async_1_Object____ContObject___ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_StartImmediate$ = (function(workflow,cancellationToken)
{
    var _2763;
    if ((cancellationToken.Tag == 1.000000)) 
    {
      var v = Option__GetValue$CancellationToken_CancellationToken_(cancellationToken);
      _2763 = v;
    }
    else
    {
      _2763 = (new CancellationToken___ctor$({Tag: 0.000000}));
    };
    var token = _2763;
    var f = workflow.Item;
    var aux = (new AsyncParamsAux___ctor$({contents: 0}, (function(value)
    {
      var ignored0 = value;
    }), (function(value)
    {
      var ignored0 = value;
    }), token));
    return f((new AsyncParams_1_Unit___ctor$Unit_((function(value)
    {
      var ignored0 = value;
    }), aux)));
});
Async_1_String____ContString___ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_Unit__ContUnit_ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async_1_WebResponse__ContWebResponse_ = (function(Item)
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Cont";
    __this.Item = Item;
});
Async__get_async$ = (function()
{
    return (new AsyncBuilder___ctor$());
});
Async__invokeCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$Object___Object___ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$String___String___ = (function(k,value)
{
    return k.Cont(value);
});
Async__invokeCont$Unit_Unit_ = (function(k,value)
{
    return k.Cont(value);
});
Async__protectedCont$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___ = (function(f)
{
    return (new Async_1_FSharpOption_1_Result_1_Error____ContFSharpOption_1_Result_1_Error___((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_ = (function(f)
{
    return (new Async_1_FSharpOption_1_Result_1_GeneratorData__ContFSharpOption_1_Result_1_GeneratorData_((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_ = (function(f)
{
    return (new Async_1_FSharpOption_1_Result_1_NavigateData__ContFSharpOption_1_Result_1_NavigateData_((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_ = (function(f)
{
    return (new Async_1_FSharpOption_1_Result_1_Unit__ContFSharpOption_1_Result_1_Unit_((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$Object___Object___ = (function(f)
{
    return (new Async_1_Object____ContObject___((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$String___String___ = (function(f)
{
    return (new Async_1_String____ContString___((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$Unit_Unit_ = (function(f)
{
    return (new Async_1_Unit__ContUnit_((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
Async__protectedCont$WebResponse_WebResponse_ = (function(f)
{
    return (new Async_1_WebResponse__ContWebResponse_((function(args)
    {
      CancellationToken__ThrowIfCancellationRequested$(args.Aux.CancellationToken);
      args.Aux.StackCounter.contents = (args.Aux.StackCounter.contents + 1);
      null;
      if ((args.Aux.StackCounter.contents > 1000)) 
      {
        args.Aux.StackCounter.contents = 0;
        null;
        return window.setTimeout((function(unitVar0)
        {
          try
          {
            return f(args);
          }
          catch(ex){
            return args.Aux.ExceptionCont(ex);
          };
        }), 1.000000);
      }
      else
      {
        try
        {
          return f(args);
        }
        catch(ex){
          return args.Aux.ExceptionCont(ex);
        };
      };
    })));
});
CancellationToken__ThrowIfCancellationRequested$ = (function(x,unitVar1)
{
    var matchValue = x.Cell;
    if ((matchValue.Tag == 1.000000)) 
    {
      var cell = Option__GetValue$FSharpRef_1_Boolean_FSharpRef_1_Boolean_(matchValue);
      if (cell.contents) 
      {
        var _cell = Option__GetValue$FSharpRef_1_Boolean_FSharpRef_1_Boolean_(matchValue);
        throw ("OperationCancelledException");
        return null;
      }
      else
      {
        ;
      };
    }
    else
    {
      ;
    };
});
CancellationToken___ctor$ = (function(Cell)
{
    var __this = this;
    __this.Cell = Cell;
});
Control__Async_StartAsPromise_Static$Object_Object_ = (function(computation)
{
    return new Promise(function(resolve,reject){(function(unitVar0)
    {
      return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
      {
        return AsyncBuilder__Bind$Object____Unit_Object____Unit_(Async__get_async$(), computation, (function(_arg1)
        {
          var res = _arg1;
          resolve(res);
          return AsyncBuilder__Zero$(Async__get_async$());
        }));
      })), {Tag: 0.000000});
    })()});
});
Control__CommandRegistry_subscribe$ = (function(x,name,command,func)
{
    return (x.add(name, command, func));
});
Control__IConfig_onDidChange$ConfigChange_1_Boolean_ConfigChange_1_Boolean_ = (function(x,keyPath,callback)
{
    return (x.onDidChange(keyPath, callback));
});
CreateEnumerable_1_Disposable___ctor$Disposable_ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerable_1_Tuple_2_IPane__Object_ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_IPane___ctor$IPane_ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_Object_____ctor$Object___ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_Object___ctor$Object_ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_String_____ctor$String___ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
CreateEnumerable_1_Tuple_2_IPane__Object___ctor$Tuple_2_IPane__Object_ = (function(factory)
{
    var __this = this;
    {};
    __this.factory = factory;
});
DateTime__ToLongTimeString$ = (function(dt,unitVar1)
{
    return dt['to'+"LocaleTime"+'String']();
});
DateTime__createUnsafe$ = (function(value,kind)
{
    var date = value == null ? new Date() : new Date(value);
    if (isNaN(date)) { throw "The string was not recognized as a valid DateTime." }
    date.kind = kind;
    return date;
});
DateTime__get_Now$ = (function(unitVar0)
{
    return DateTime__createUnsafe$(null, 2);
});
DateTime__get_Ticks$ = (function(dt,unitVar1)
{
    return ((dt.getTime() + 62135604000000.000000) * TimeSpan__get_TicksPerMillisecond$());
});
ErrorLinterProvider__create$ = (function(unitVar0)
{
    var grammarScopes = ["source.texton"];
    var scope = "file";
    var lint = (function(editor)
    {
      return ErrorLinterProvider__lint$(editor);
    });
    return [(new Provider___ctor$(grammarScopes, scope, true, lint))];
});
ErrorLinterProvider__lint$ = (function(editor)
{
    return Control__Async_StartAsPromise_Static$Object_Object_(AsyncBuilder__Delay$Object___Object___(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_Error____Object___FSharpOption_1_Result_1_Error____Object___(Async__get_async$(), LanguageService__parseEditor$(editor), (function(_arg1)
      {
        var result = _arg1;
        var result_ = {Tag: 0.000000};
        var linter = (((window.atom).config).get("texton.UseLinter"));
        var _5727;
        var matchValue = (new TupleFSharpOption_1_Result_1_Error____FSharpOption_1_Result_1_LintWarning___(result, result_));
        if ((matchValue.Items[0.000000].Tag == 0.000000)) 
        {
          if ((matchValue.Items[1.000000].Tag == 0.000000)) 
          {
            _5727 = [];
          }
          else
          {
            var n = Option__GetValue$Result_1_LintWarning___Result_1_LintWarning___(matchValue.Items[1.000000]);
            if (linter) 
            {
              _5727 = Array__Map$LintWarning__Object_LintWarning__Object_((function(item)
              {
                return ErrorLinterProvider__mapLint$(editor, item);
              }), n.Data);
            }
            else
            {
              _5727 = [];
            };
          };
        }
        else
        {
          if ((matchValue.Items[1.000000].Tag == 0.000000)) 
          {
            var _n = Option__GetValue$Result_1_Error___Result_1_Error___(matchValue.Items[0.000000]);
            _5727 = Array__Map$Error__Object_Error__Object_((function(item)
            {
              return ErrorLinterProvider__mapError$(editor, item);
            }), _n.Data);
          }
          else
          {
            var __n = Option__GetValue$Result_1_Error___Result_1_Error___(matchValue.Items[0.000000]);
            var n_ = Option__GetValue$Result_1_LintWarning___Result_1_LintWarning___(matchValue.Items[1.000000]);
            var r = Array__Map$Error__Object_Error__Object_((function(item)
            {
              return ErrorLinterProvider__mapError$(editor, item);
            }), __n.Data);
            var _5911;
            if (linter) 
            {
              _5911 = Array__Map$LintWarning__Object_LintWarning__Object_((function(item)
              {
                return ErrorLinterProvider__mapLint$(editor, item);
              }), n_.Data);
            }
            else
            {
              _5911 = [];
            };
            var r_ = _5911;
            _5727 = Array__Concat$Object_Object_(Seq__OfList$Object___Object___(List__CreateCons$Object___Object___(r, List__CreateCons$Object___Object___(r_, List__Empty$Object___Object___()))));
          };
        };
        return AsyncBuilder__Return$Object___Object___(Async__get_async$(), _5727);
      }));
    })));
});
ErrorLinterProvider__mapError$ = (function(editor,item)
{
    var range = [[(item.StartLine - 1), (item.StartColumn - 1)], [(item.EndLine - 1), item.EndColumn]];
    var error = (new LintResult___ctor$(item.Severity, String__Replace$(item.Message, "\n", ""), (((editor.buffer).file).path), range));
    Logger__logf$("Service", "Got error %A", [error]);
    return error;
});
ErrorLinterProvider__mapLint$ = (function(editor,item)
{
    var range = [[(item.StartLine - 1), (item.StartColumn - 1)], [(item.EndLine - 1), item.EndColumn]];
    return (new LintResult___ctor$("Trace", String__Replace$(item.Info, "\n", ""), (((editor.buffer).file).path), range));
});
GenerateRequest___ctor$ = (function(Config)
{
    var __this = this;
    __this.Config = Config;
});
GeneratorConfiguration___ctor$ = (function(NumSpacesBetweenSentences,NumBlankLinesBetweenParagraphs,WindowsLineEndings)
{
    var __this = this;
    __this.NumSpacesBetweenSentences = NumSpacesBetweenSentences;
    __this.NumBlankLinesBetweenParagraphs = NumBlankLinesBetweenParagraphs;
    __this.WindowsLineEndings = WindowsLineEndings;
});
GeneratorPane__copyToClipboard$ = (function(output)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      var folder = (function(x)
      {
        return (function(y)
        {
          return (x + y);
        });
      });
      var state = "";
      (((window.atom).clipboard).write((function(array)
      {
        return Array__Fold$String_1_String_1String_String(folder, state, array);
      })(Array__Map$OutputString__String_1OutputString__String((function(o)
      {
        return o.Value;
      }), output))));
      var _4966;
      return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4966);
    })), {Tag: 0.000000});
});
GeneratorPane__divContent$ = (function(unitVar0)
{
    return (TextOnViewHelpers__jq$("\u003cdiv /\u003e").addClass("content"));
});
GeneratorPane__divInsetPadded$ = (function(unitVar0)
{
    return TextOnViewHelpers__jq$("\u003cdiv class=\u0027inset-panel padded\u0027/\u003e");
});
GeneratorPane__divPadded$ = (function(unitVar0)
{
    return TextOnViewHelpers__jq$("\u003cdiv class=\u0027padded\u0027/\u003e");
});
GeneratorPane__getTextOnCursorLine$ = (function(unitVar0)
{
    var editor = (((window.atom).workspace).getActiveTextEditor());
    var posn = (editor.getCursorBufferPosition());
    return (posn.row);
});
GeneratorPane__makeAttributes$ = (function(res)
{
    var rows = Array__Map$GeneratorAttribute__JQuery_GeneratorAttribute__JQuery_((function(att)
    {
      return ((TextOnViewHelpers__jq$("\u003ctr /\u003e").append((GeneratorPane__td$().append(GeneratorPane__makeLinkForAttribute$(res.FileName, att.Name))))).append((GeneratorPane__td$().append(GeneratorPane__makeCombobox$String_1String("Attribute", att.Name, att.Value, List__OfArray$String_1String(att.Suggestions))))));
    }), res.Attributes);
    var topRow = ((TextOnViewHelpers__jq$("\u003ctr /\u003e").append((GeneratorPane__th$().append("Name")))).append((GeneratorPane__th$().append("Value"))));
    var folder = (function(q)
    {
      return (function(r)
      {
        return (q.append(r));
      });
    });
    var state = TextOnViewHelpers__jq$("\u003ctbody /\u003e");
    var tbody = (function(array)
    {
      return Array__Fold$JQuery__JQuery_JQuery__JQuery_(folder, state, array);
    })(rows);
    return ((TextOnViewHelpers__jq$("\u003ctable /\u003e").append((TextOnViewHelpers__jq$("\u003cthead /\u003e").append(topRow)))).append(tbody));
});
GeneratorPane__makeCombobox$String_1String = (function(ty,name,value,options)
{
    var folder = (function(q)
    {
      return (function(o)
      {
        return (q.append((("\u003coption\u003e" + o) + "\u003c/option\u003e")));
      });
    });
    var state = TextOnViewHelpers__jq$((("\u003cselect id=\"" + name) + "\" /\u003e"));
    var q = (function(list)
    {
      return List__Fold$String_1_JQuery_String_JQuery_(folder, state, list);
    })(options);
    return (q.change((function(o)
    {
      var _value = (q.val());
      return GeneratorPane__valueSet$(ty, name, _value);
    })));
});
GeneratorPane__makeLinkForAttribute$ = (function(fileName,attribute)
{
    return ((TextOnViewHelpers__jq$("\u003ca /\u003e").append(attribute)).click((function(_arg1)
    {
      return GeneratorPane__navigateToAttribute$(fileName, attribute);
    })));
});
GeneratorPane__makeLinkForLine$ = (function(file,line,text)
{
    return ((TextOnViewHelpers__jq$("\u003ca /\u003e").append(text)).click((function(_arg3)
    {
      return GeneratorPane__navigateToFileLine$(file, line);
    })));
});
GeneratorPane__makeLinkForVariable$ = (function(fileName,variable)
{
    return ((TextOnViewHelpers__jq$("\u003ca /\u003e").append(variable)).click((function(_arg2)
    {
      return GeneratorPane__navigateToVariable$(fileName, variable);
    })));
});
GeneratorPane__makeTitle$ = (function(res)
{
    var link = ((TextOnViewHelpers__jq$("\u003ca /\u003e").append(res.FunctionName)).click((function(_arg1)
    {
      return GeneratorPane__navigateToFunction$(res.FileName, res.FunctionName);
    })));
    return (TextOnViewHelpers__jq$("\u003ch1\u003eGenerator for \u003c/h1\u003e").append(link));
});
GeneratorPane__makeVariables$ = (function(res)
{
    var rows = Array__Map$GeneratorVariable__JQuery_GeneratorVariable__JQuery_((function(variable)
    {
      return (((TextOnViewHelpers__jq$("\u003ctr /\u003e").append((GeneratorPane__td$().append(GeneratorPane__makeLinkForVariable$(res.FileName, variable.Name))))).append((GeneratorPane__td$().append(variable.Text)))).append((GeneratorPane__td$().append(GeneratorPane__makeCombobox$String_1String("Variable", variable.Name, variable.Value, List__OfArray$String_1String(variable.Suggestions))))));
    }), res.Variables);
    var topRow = (((TextOnViewHelpers__jq$("\u003ctr /\u003e").append((GeneratorPane__th$().append("Name")))).append((GeneratorPane__th$().append("Text")))).append((GeneratorPane__th$().append("Value"))));
    var folder = (function(q)
    {
      return (function(r)
      {
        return (q.append(r));
      });
    });
    var state = TextOnViewHelpers__jq$("\u003ctbody /\u003e");
    var tbody = (function(array)
    {
      return Array__Fold$JQuery__JQuery_JQuery__JQuery_(folder, state, array);
    })(rows);
    return ((TextOnViewHelpers__jq$("\u003ctable /\u003e").append((TextOnViewHelpers__jq$("\u003cthead /\u003e").append(topRow)))).append(tbody));
});
GeneratorPane__navigateToAttribute$ = (function(fileName,attributeName)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_NavigateData__Unit_FSharpOption_1_Result_1_NavigateData__Unit_(Async__get_async$(), LanguageService__navigateToAttribute$(fileName, attributeName), (function(_arg1)
      {
        var navigationResult = _arg1;
        var _3991;
        if (Option__IsSome$Result_1_NavigateData_Result_1_NavigateData_(navigationResult)) 
        {
          var data = Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_(navigationResult).Data;
          GeneratorPane__navigateToEditor$(data.FileName, (data.LineNumber - 1), (data.Location - 1));
          _3991 = AsyncBuilder__Zero$(Async__get_async$());
        }
        else
        {
          _3991 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _3991, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _4028;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4028);
        })));
      }));
    })), {Tag: 0.000000});
});
GeneratorPane__navigateToEditor$ = (function(file,line,col)
{
    var found = false;
    var arr = (((window.atom).workspace).getPanes());
    for (var _3840 = 0; _3840 <= (Array__Length$IPane_IPane_(arr) - 1); _3840++)
    {
      (function(idx)
      {
        var pane = arr[idx];
        if ((!found)) 
        {
          var _arr = (pane.getItems());
          for (var _3839 = 0; _3839 <= (Array__Length$Object_Object_(_arr) - 1); _3839++)
          {
            (function(_idx)
            {
              var item = _arr[_idx];
              if ((!found)) 
              {
                try
                {
                  var ed = item;
                  if (((ed.getPath()) == file)) 
                  {
                    (pane.activate());
                    var ignored0 = (pane.activateItem(ed));
                    var _ignored0 = (ed.setCursorBufferPosition([line, col]));
                    found = true;
                    null;
                  }
                  else
                  {
                    ;
                  };
                }
                catch(matchValue){
                  ;
                };
              }
              else
              {
                ;
              };
            })(_3839);
          };
        }
        else
        {
          ;
        };
      })(_3840);
    };
    if ((!found)) 
    {
      var ignored0 = (((window.atom).workspace).open(file, (new OpenOptions___ctor$(line, col))));
    }
    else
    {
      ;
    };
});
GeneratorPane__navigateToFileLine$ = (function(file,line)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      GeneratorPane__navigateToEditor$(file, (line - 1), 0);
      return AsyncBuilder__Zero$(Async__get_async$());
    })), {Tag: 0.000000});
});
GeneratorPane__navigateToFunction$ = (function(fileName,functionName)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_NavigateData__Unit_FSharpOption_1_Result_1_NavigateData__Unit_(Async__get_async$(), LanguageService__navigateToFunction$(fileName, functionName), (function(_arg1)
      {
        var navigationResult = _arg1;
        var _3768;
        if (Option__IsSome$Result_1_NavigateData_Result_1_NavigateData_(navigationResult)) 
        {
          var data = Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_(navigationResult).Data;
          GeneratorPane__navigateToEditor$(data.FileName, (data.LineNumber - 1), (data.Location - 1));
          _3768 = AsyncBuilder__Zero$(Async__get_async$());
        }
        else
        {
          _3768 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _3768, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _3870;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _3870);
        })));
      }));
    })), {Tag: 0.000000});
});
GeneratorPane__navigateToVariable$ = (function(fileName,variableName)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_NavigateData__Unit_FSharpOption_1_Result_1_NavigateData__Unit_(Async__get_async$(), LanguageService__navigateToVariable$(fileName, variableName), (function(_arg1)
      {
        var navigationResult = _arg1;
        var _4426;
        if (Option__IsSome$Result_1_NavigateData_Result_1_NavigateData_(navigationResult)) 
        {
          var data = Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_(navigationResult).Data;
          GeneratorPane__navigateToEditor$(data.FileName, (data.LineNumber - 1), (data.Location - 1));
          _4426 = AsyncBuilder__Zero$(Async__get_async$());
        }
        else
        {
          _4426 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _4426, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _4463;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4463);
        })));
      }));
    })), {Tag: 0.000000});
});
GeneratorPane__openTextOnGeneratorPane$ = (function(closeIfNotFound)
{
    return Async_1_FromContinuations$Unit_Unit_((function(tupledArg)
    {
      var cont = tupledArg.Items[0.000000];
      var econt = tupledArg.Items[1.000000];
      var ccont = tupledArg.Items[2.000000];
      var prevPane = (((window.atom).workspace).getActivePane());
      var prevItem = (prevPane.getActiveItem());
      var activateAndCont = (function(unitVar0)
      {
        var ignored0 = (prevPane.activate());
        var _ignored0 = (prevPane.activateItem(prevItem));
        var _742;
        return cont(_742);
      });
      var matchValue = GeneratorPane__tryFindTextOnGeneratorPane$(closeIfNotFound);
      if ((matchValue.Tag == 0.000000)) 
      {
        if ((!closeIfNotFound)) 
        {
          return ((((window.atom).workspace).open("TextOn Generator", (new OpenEditorOptions___ctor$("right")))).done((function(ed)
          {
            var _2614;
            return activateAndCont(_2614);
          })));
        }
        else
        {
          ;
        };
      }
      else
      {
        var pane = Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(matchValue).Items[0.000000];
        var item = Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(matchValue).Items[1.000000];
        var ignored0 = (pane.activateItem(item));
        var _ignored0 = (pane.activate());
        var _2629;
        return activateAndCont(_2629);
      };
    }));
});
GeneratorPane__performGeneration$ = (function(unitVar0)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_(Async__get_async$(), LanguageService__generate$(), (function(_arg3)
      {
        var generatorData = _arg3;
        var _4658;
        if (Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData)) 
        {
          var data = Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData).Data;
          _4658 = AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__replaceTextOnGeneratorHtmlPanel$(data), (function(_arg4)
          {
            var _4679;
            return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4679);
          }));
        }
        else
        {
          _4658 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _4658, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _4691;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4691);
        })));
      }));
    })), {Tag: 0.000000});
});
GeneratorPane__replaceTextOnGeneratorHtmlPanel$ = (function(res)
{
    return AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      var ignored0 = (TextOnViewHelpers__jq$(".texton").empty());
      var identity = (function(unitVar0)
      {
        var copyOfStruct = DateTime__get_Now$();
        return ("html" + DateTime__get_Ticks$(copyOfStruct).toString());
      });
      var title = (GeneratorPane__divContent$().append(GeneratorPane__makeTitle$(res)));
      var paddedTitle = (GeneratorPane__divInsetPadded$().append(title));
      var attributes = ((GeneratorPane__divContent$().append("\u003ch2\u003eAttributes\u003c/h2\u003e")).append(GeneratorPane__makeAttributes$(res)));
      var paddedAttributes = (TextOnViewHelpers__jq$("\u003cdiv class=\u0027inset-panel padded\u0027/\u003e").append(attributes));
      var variables = ((GeneratorPane__divContent$().append("\u003ch2\u003eVariables\u003c/h2\u003e")).append(GeneratorPane__makeVariables$(res)));
      var paddedVariables = (GeneratorPane__divInsetPadded$().append(variables));
      var makePaddedGeneratorButton = (function(unitVar0)
      {
        var button = (TextOnViewHelpers__jq$("\u003cbutton\u003eGenerate\u003c/button\u003e").click((function(_arg4)
        {
          return GeneratorPane__performGeneration$();
        })));
        var generatorButton = ((GeneratorPane__divContent$().append("\u003ch2\u003eGenerate\u003c/h2\u003e")).append(button));
        return (GeneratorPane__divInsetPadded$().append(generatorButton));
      });
      var makePaddedGeneratorOutput = (function(output)
      {
        var folder = (function(q)
        {
          return (function(q2)
          {
            return (q.append(q2));
          });
        });
        var state = TextOnViewHelpers__jq$("\u003cdiv /\u003e");
        var _folder = (function(tupledArg)
        {
          var q = tupledArg.Items[0.000000];
          var li = tupledArg.Items[1.000000];
          return (function(t)
          {
            if (t.IsPb) 
            {
              return (new TupleJQuery__FSharpList_1_JQuery_(TextOnViewHelpers__jq$("\u003cp /\u003e"), List__CreateCons$JQuery_JQuery_(q, li)));
            }
            else
            {
              return (new TupleJQuery__FSharpList_1_JQuery_((q.append(GeneratorPane__makeLinkForLine$(t.File, t.LineNumber, t.Value))), li));
            };
          });
        });
        var _state = (new TupleJQuery__FSharpList_1_JQuery_(TextOnViewHelpers__jq$("\u003cp /\u003e"), List__Empty$JQuery_JQuery_()));
        var outputHtml = (function(list)
        {
          return List__Fold$JQuery__JQuery_JQuery__JQuery_(folder, state, list);
        })((function(tupledArg)
        {
          var q = tupledArg.Items[0.000000];
          var li = tupledArg.Items[1.000000];
          return List__Reverse$JQuery_JQuery_(List__CreateCons$JQuery_JQuery_(q, li));
        })((function(array)
        {
          return Array__Fold$OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_OutputString__Tuple_2_JQuery__FSharpList_1_JQuery_(_folder, _state, array);
        })(output)));
        var paddedOutput = ((GeneratorPane__divContent$().append("\u003ch2\u003eOutput\u003c/h2\u003e")).append(outputHtml));
        var copyToClipboardButton = (TextOnViewHelpers__jq$("\u003cbutton\u003eCopy to Clipboard\u003c/button\u003e").click((function(_arg5)
        {
          return GeneratorPane__copyToClipboard$(output);
        })));
        var paddedCopyToClipboardButton = (GeneratorPane__divContent$().append(copyToClipboardButton));
        return ((GeneratorPane__divInsetPadded$().append(paddedOutput)).append(paddedCopyToClipboardButton));
      });
      var _4986;
      var q = ((((TextOnViewHelpers__jq$((("\u003catom-panel id=\u0027" + identity(_4986)) + "\u0027 /\u003e")).addClass("top texton-block texton-html-block")).append((GeneratorPane__divPadded$().append(paddedTitle)))).append((GeneratorPane__divPadded$().append(paddedAttributes)))).append((GeneratorPane__divPadded$().append(paddedVariables))));
      var _4999;
      if (res.CanGenerate) 
      {
        var _5008;
        _4999 = (q.append((GeneratorPane__divPadded$().append(makePaddedGeneratorButton(_5008)))));
      }
      else
      {
        _4999 = q;
      };
      var _q = _4999;
      var _5009;
      if ((Array__BoxedLength$(res.Output) > 0)) 
      {
        _5009 = (_q.append((GeneratorPane__divPadded$().append(makePaddedGeneratorOutput(res.Output)))));
      }
      else
      {
        _5009 = _q;
      };
      var __q = _5009;
      var _ignored0 = (__q.appendTo(TextOnViewHelpers__jq$(".texton")));
      return AsyncBuilder__Zero$(Async__get_async$());
    }));
});
GeneratorPane__sendToGenerator$ = (function(res)
{
    return AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__replaceTextOnGeneratorHtmlPanel$(res), (function(_arg5)
      {
        var ignored0 = (TextOnViewHelpers__jq$(".texton").scrollTop(99999999.000000));
        return AsyncBuilder__Zero$(Async__get_async$());
      }));
    }));
});
GeneratorPane__sendToTextOnGenerator$ = (function(closeIfNotFound)
{
    return AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      var editor = (((window.atom).workspace).getActiveTextEditor());
      if (TextOnViewHelpers__isTextOnEditor$(editor)) 
      {
        var line = GeneratorPane__getTextOnCursorLine$();
        return AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__openTextOnGeneratorPane$(closeIfNotFound), (function(_arg1)
        {
          return AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_(Async__get_async$(), LanguageService__generatorStart$(editor, line), (function(_arg2)
          {
            var res = _arg2;
            if (Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_(res)) 
            {
              return AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__sendToGenerator$(Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_(res).Data), (function(_arg3)
              {
                var _5044;
                return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _5044);
              }));
            }
            else
            {
              return AsyncBuilder__Zero$(Async__get_async$());
            };
          }));
        }));
      }
      else
      {
        return AsyncBuilder__Zero$(Async__get_async$());
      };
    }));
});
GeneratorPane__td$ = (function(unitVar0)
{
    return (TextOnViewHelpers__jq$("\u003ctd /\u003e").addClass("textontablecell"));
});
GeneratorPane__th$ = (function(unitVar0)
{
    return (TextOnViewHelpers__jq$("\u003cth /\u003e").addClass("textontablecell"));
});
GeneratorPane__tryFindTextOnGeneratorPane$ = (function(closeIfNotFound)
{
    var panes = (((window.atom).workspace).getPanes());
    return (function(o)
    {
      if (Option__IsSome$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(o)) 
      {
        return o;
      }
      else
      {
        if (closeIfNotFound) 
        {
          LanguageService__generatorStop$();
        }
        else
        {
          ;
        };
        return {Tag: 0.000000};
      };
    })(List__TryPick$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(arg0)
    {
      return {Tag: 1.000000, Value: arg0};
    }), Seq__ToList$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(Seq__Delay$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar)
    {
      return Seq__Collect$IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(pane)
      {
        return Seq__Collect$Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(item)
        {
          if ((item.getTitle() == "TextOn Generator")) 
          {
            return Seq__Singleton$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((new TupleIPane__Object_(pane, item)));
          }
          else
          {
            return Seq__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_();
          };
        }), Seq__OfArray$Object_Object_((pane.getItems())));
      }), Seq__OfArray$IPane_IPane_(panes));
    })))));
});
GeneratorPane__valueSet$ = (function(ty,name,value)
{
    return Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_(Async__get_async$(), LanguageService__generatorValueSet$(ty, name, value), (function(_arg1)
      {
        var generatorData = _arg1;
        var _4229;
        if (Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData)) 
        {
          var data = Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData).Data;
          _4229 = AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__replaceTextOnGeneratorHtmlPanel$(data), (function(_arg2)
          {
            var _4250;
            return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4250);
          }));
        }
        else
        {
          _4229 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _4229, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _4262;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _4262);
        })));
      }));
    })), {Tag: 0.000000});
});
GeneratorStartRequest___ctor$ = (function(FileName,LineNumber,Lines)
{
    var __this = this;
    __this.FileName = FileName;
    __this.LineNumber = LineNumber;
    __this.Lines = Lines;
});
GeneratorStopRequest___ctor$ = (function(Blank)
{
    var __this = this;
    __this.Blank = Blank;
});
GeneratorValueSetRequest___ctor$ = (function(Type,Name,Value)
{
    var __this = this;
    __this.Type = Type;
    __this.Name = Name;
    __this.Value = Value;
});
LanguageService__genPort$ = (function(unitVar0)
{
    var r = ((window.Math).random());
    var r_ = ((r * (8999.000000 - 8100.000000)) + 8100.000000);
    return r_.toString().substring(0, 0 + 4);
});
LanguageService__generate$ = (function(unitVar0)
{
    var config = (new GeneratorConfiguration___ctor$((((window.atom).config).get("texton.GeneratorConfig.NumSpacesBetweenSentences")), (((window.atom).config).get("texton.GeneratorConfig.NumBlankLinesBetweenParagraphs")), (((window.atom).config).get("texton.GeneratorConfig.WindowsLineEndings"))));
    return LanguageService__send$GeneratorData_GeneratorData_(0, LanguageService__request$GenerateRequest_GenerateRequest_(LanguageService__url$("generate"), (new GenerateRequest___ctor$(config))));
});
LanguageService__generatorStart$ = (function(editor,lineNumber)
{
    if ((TextOnViewHelpers__isTextOnEditor$(editor) && (((editor.buffer).file) != null))) 
    {
      var path = (((editor.buffer).file).path);
      var text = (editor.getText());
      var lines = String__SplitWithoutOptions$(String__Replace$(text, "﻿", ""), ["\n"]);
      return LanguageService__send$GeneratorData_GeneratorData_(0, LanguageService__request$GeneratorStartRequest_GeneratorStartRequest_(LanguageService__url$("generatorstart"), (new GeneratorStartRequest___ctor$(path, lineNumber, lines))));
    }
    else
    {
      return AsyncBuilder__Delay$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), (function(unitVar)
      {
        return AsyncBuilder__Return$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), {Tag: 0.000000});
      }));
    };
});
LanguageService__generatorStop$ = (function(unitVar0)
{
    var ignored0 = LanguageService__send$Unit_Unit_(0, LanguageService__request$GeneratorStopRequest_GeneratorStopRequest_(LanguageService__url$("generatorstop"), (new GeneratorStopRequest___ctor$(""))));
});
LanguageService__generatorValueSet$ = (function(ty,name,value)
{
    return LanguageService__send$GeneratorData_GeneratorData_(0, LanguageService__request$GeneratorValueSetRequest_GeneratorValueSetRequest_(LanguageService__url$("generatorvalueset"), (new GeneratorValueSetRequest___ctor$(ty, name, value))));
});
LanguageService__get_port$ = (function()
{
    return LanguageService__genPort$();
});
LanguageService__get_service$ = (function()
{
    return {Tag: 0.000000};
});
LanguageService__navigateToAttribute$ = (function(fileName,attributeName)
{
    return LanguageService__send$NavigateData_NavigateData_(0, LanguageService__request$NavigateRequest_NavigateRequest_(LanguageService__url$("navigaterequest"), (new NavigateRequest___ctor$(fileName, "NavigateToAttribute", attributeName))));
});
LanguageService__navigateToFunction$ = (function(fileName,functionName)
{
    return LanguageService__send$NavigateData_NavigateData_(0, LanguageService__request$NavigateRequest_NavigateRequest_(LanguageService__url$("navigaterequest"), (new NavigateRequest___ctor$(fileName, "NavigateToFunction", functionName))));
});
LanguageService__navigateToVariable$ = (function(fileName,variableName)
{
    return LanguageService__send$NavigateData_NavigateData_(0, LanguageService__request$NavigateRequest_NavigateRequest_(LanguageService__url$("navigaterequest"), (new NavigateRequest___ctor$(fileName, "NavigateToVariable", variableName))));
});
LanguageService__parse$ = (function(path,text)
{
    var lines = String__SplitWithoutOptions$(String__Replace$(text, "﻿", ""), ["\n"]);
    return LanguageService__send$Error___Error___(0, LanguageService__request$ParseRequest_ParseRequest_(LanguageService__url$("parse"), (new ParseRequest___ctor$(path, true, lines))));
});
LanguageService__parseEditor$ = (function(editor)
{
    if ((TextOnViewHelpers__isTextOnEditor$(editor) && (((editor.buffer).file) != null))) 
    {
      var path = (((editor.buffer).file).path);
      var text = (editor.getText());
      return LanguageService__parse$(path, text);
    }
    else
    {
      return AsyncBuilder__Delay$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), (function(unitVar)
      {
        return AsyncBuilder__Return$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), {Tag: 0.000000});
      }));
    };
});
LanguageService__parseResponse$Error___Error___ = (function(response)
{
    return Array__Map$String_1_FSharpOption_1_Result_1_Error___String_FSharpOption_1_Result_1_Error___((function(s)
    {
      var matchValue = LanguageService__tryParse$Result_1_Error___Result_1_Error___(s);
      if ((matchValue.Tag == 1.000000)) 
      {
        var event = Option__GetValue$Result_1_Error___Result_1_Error___(matchValue);
        var o = event;
        Logger__logf$("Service", "Got \u0027%s\u0027: %O", [event.Kind, o]);
        var _matchValue = event.Kind;
        if ((_matchValue == "errors")) 
        {
          return {Tag: 1.000000, Value: event};
        }
        else
        {
          if ((_matchValue == "lint")) 
          {
            return {Tag: 1.000000, Value: event};
          }
          else
          {
            if ((_matchValue == "generatorSetup")) 
            {
              return {Tag: 1.000000, Value: event};
            }
            else
            {
              if ((_matchValue == "navigate")) 
              {
                return {Tag: 1.000000, Value: event};
              }
              else
              {
                if ((_matchValue == "error")) 
                {
                  Logger__logf$("Service", "Received error event \u0027%s\u0027: %O", [s, o]);
                  return {Tag: 0.000000};
                }
                else
                {
                  if ((_matchValue == "info")) 
                  {
                    Logger__logf$("Service", "Received info event \u0027%s\u0027: %O", [s, o]);
                    return {Tag: 0.000000};
                  }
                  else
                  {
                    var _s = _matchValue;
                    Logger__logf$("Service", "Received unexpected event \u0027%s\u0027: %O", [_s, o]);
                    return {Tag: 0.000000};
                  };
                };
              };
            };
          };
        };
      }
      else
      {
        Logger__logf$("Service", "Invalid response from TextOn: %s", [s]);
        return {Tag: 0.000000};
      };
    }), response);
});
LanguageService__parseResponse$GeneratorData_GeneratorData_ = (function(response)
{
    return Array__Map$String_1_FSharpOption_1_Result_1_GeneratorData_String_FSharpOption_1_Result_1_GeneratorData_((function(s)
    {
      var matchValue = LanguageService__tryParse$Result_1_GeneratorData_Result_1_GeneratorData_(s);
      if ((matchValue.Tag == 1.000000)) 
      {
        var event = Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_(matchValue);
        var o = event;
        Logger__logf$("Service", "Got \u0027%s\u0027: %O", [event.Kind, o]);
        var _matchValue = event.Kind;
        if ((_matchValue == "errors")) 
        {
          return {Tag: 1.000000, Value: event};
        }
        else
        {
          if ((_matchValue == "lint")) 
          {
            return {Tag: 1.000000, Value: event};
          }
          else
          {
            if ((_matchValue == "generatorSetup")) 
            {
              return {Tag: 1.000000, Value: event};
            }
            else
            {
              if ((_matchValue == "navigate")) 
              {
                return {Tag: 1.000000, Value: event};
              }
              else
              {
                if ((_matchValue == "error")) 
                {
                  Logger__logf$("Service", "Received error event \u0027%s\u0027: %O", [s, o]);
                  return {Tag: 0.000000};
                }
                else
                {
                  if ((_matchValue == "info")) 
                  {
                    Logger__logf$("Service", "Received info event \u0027%s\u0027: %O", [s, o]);
                    return {Tag: 0.000000};
                  }
                  else
                  {
                    var _s = _matchValue;
                    Logger__logf$("Service", "Received unexpected event \u0027%s\u0027: %O", [_s, o]);
                    return {Tag: 0.000000};
                  };
                };
              };
            };
          };
        };
      }
      else
      {
        Logger__logf$("Service", "Invalid response from TextOn: %s", [s]);
        return {Tag: 0.000000};
      };
    }), response);
});
LanguageService__parseResponse$NavigateData_NavigateData_ = (function(response)
{
    return Array__Map$String_1_FSharpOption_1_Result_1_NavigateData_String_FSharpOption_1_Result_1_NavigateData_((function(s)
    {
      var matchValue = LanguageService__tryParse$Result_1_NavigateData_Result_1_NavigateData_(s);
      if ((matchValue.Tag == 1.000000)) 
      {
        var event = Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_(matchValue);
        var o = event;
        Logger__logf$("Service", "Got \u0027%s\u0027: %O", [event.Kind, o]);
        var _matchValue = event.Kind;
        if ((_matchValue == "errors")) 
        {
          return {Tag: 1.000000, Value: event};
        }
        else
        {
          if ((_matchValue == "lint")) 
          {
            return {Tag: 1.000000, Value: event};
          }
          else
          {
            if ((_matchValue == "generatorSetup")) 
            {
              return {Tag: 1.000000, Value: event};
            }
            else
            {
              if ((_matchValue == "navigate")) 
              {
                return {Tag: 1.000000, Value: event};
              }
              else
              {
                if ((_matchValue == "error")) 
                {
                  Logger__logf$("Service", "Received error event \u0027%s\u0027: %O", [s, o]);
                  return {Tag: 0.000000};
                }
                else
                {
                  if ((_matchValue == "info")) 
                  {
                    Logger__logf$("Service", "Received info event \u0027%s\u0027: %O", [s, o]);
                    return {Tag: 0.000000};
                  }
                  else
                  {
                    var _s = _matchValue;
                    Logger__logf$("Service", "Received unexpected event \u0027%s\u0027: %O", [_s, o]);
                    return {Tag: 0.000000};
                  };
                };
              };
            };
          };
        };
      }
      else
      {
        Logger__logf$("Service", "Invalid response from TextOn: %s", [s]);
        return {Tag: 0.000000};
      };
    }), response);
});
LanguageService__parseResponse$Unit_Unit_ = (function(response)
{
    return Array__Map$String_1_FSharpOption_1_Result_1_Unit_String_FSharpOption_1_Result_1_Unit_((function(s)
    {
      var matchValue = LanguageService__tryParse$Result_1_Unit_Result_1_Unit_(s);
      if ((matchValue.Tag == 1.000000)) 
      {
        var event = Option__GetValue$Result_1_Unit_Result_1_Unit_(matchValue);
        var o = event;
        Logger__logf$("Service", "Got \u0027%s\u0027: %O", [event.Kind, o]);
        var _matchValue = event.Kind;
        if ((_matchValue == "errors")) 
        {
          return {Tag: 1.000000, Value: event};
        }
        else
        {
          if ((_matchValue == "lint")) 
          {
            return {Tag: 1.000000, Value: event};
          }
          else
          {
            if ((_matchValue == "generatorSetup")) 
            {
              return {Tag: 1.000000, Value: event};
            }
            else
            {
              if ((_matchValue == "navigate")) 
              {
                return {Tag: 1.000000, Value: event};
              }
              else
              {
                if ((_matchValue == "error")) 
                {
                  Logger__logf$("Service", "Received error event \u0027%s\u0027: %O", [s, o]);
                  return {Tag: 0.000000};
                }
                else
                {
                  if ((_matchValue == "info")) 
                  {
                    Logger__logf$("Service", "Received info event \u0027%s\u0027: %O", [s, o]);
                    return {Tag: 0.000000};
                  }
                  else
                  {
                    var _s = _matchValue;
                    Logger__logf$("Service", "Received unexpected event \u0027%s\u0027: %O", [_s, o]);
                    return {Tag: 0.000000};
                  };
                };
              };
            };
          };
        };
      }
      else
      {
        Logger__logf$("Service", "Invalid response from TextOn: %s", [s]);
        return {Tag: 0.000000};
      };
    }), response);
});
LanguageService__request$GenerateRequest_GenerateRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$GeneratorStartRequest_GeneratorStartRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$GeneratorStopRequest_GeneratorStopRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$GeneratorValueSetRequest_GeneratorValueSetRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$NavigateRequest_NavigateRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$ParseRequest_ParseRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__request$UpdateGeneratorRequest_UpdateGeneratorRequest_ = (function(url,data)
{
    return AsyncBuilder__Delay$String___String___(Async__get_async$(), (function(unitVar)
    {
      Logger__logf$("Service", "Sending request: %O", [data]);
      var r = WebRequest__Create$(url);
      var req = r;
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Accept", "application/json");
      WebHeaderCollection__Add$(WebRequest__get_Headers$(req), "Content-Type", "application/json");
      WebRequest__set_Method$(req, "POST");
      var str = ((window.JSON).stringify(data));
      var _data = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), str);
      var stream = WebRequest__GetRequestStream$(req);
      Stream__Write$(stream, _data, 0, Array__BoxedLength$(_data));
      return AsyncBuilder__Bind$WebResponse__String___WebResponse__String___(Async__get_async$(), WebRequest__AsyncGetResponse$(req), (function(_arg1)
      {
        var res = _arg1;
        var _stream = WebResponse__GetResponseStream$(res);
        var __data = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(_stream));
        var d = ((window.JSON).parse(__data));
        var _res = d;
        return AsyncBuilder__Return$String___String___(Async__get_async$(), _res);
      }));
    }));
});
LanguageService__send$Error___Error___ = (function(id,req)
{
    return AsyncBuilder__Delay$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__TryWith$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), AsyncBuilder__Delay$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), (function(_unitVar)
      {
        return AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Error___String____FSharpOption_1_Result_1_Error___(Async__get_async$(), req, (function(_arg1)
        {
          var r = _arg1;
          return AsyncBuilder__Return$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), LanguageService__parseResponse$Error___Error___(r)[id]);
        }));
      })), (function(_arg2)
      {
        var e = _arg2;
        Logger__logf$("ERROR", "Parsing response failed: %O", [e]);
        return AsyncBuilder__Return$FSharpOption_1_Result_1_Error___FSharpOption_1_Result_1_Error___(Async__get_async$(), {Tag: 0.000000});
      }));
    }));
});
LanguageService__send$GeneratorData_GeneratorData_ = (function(id,req)
{
    return AsyncBuilder__Delay$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__TryWith$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), AsyncBuilder__Delay$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), (function(_unitVar)
      {
        return AsyncBuilder__Bind$String____FSharpOption_1_Result_1_GeneratorData_String____FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), req, (function(_arg1)
        {
          var r = _arg1;
          return AsyncBuilder__Return$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), LanguageService__parseResponse$GeneratorData_GeneratorData_(r)[id]);
        }));
      })), (function(_arg2)
      {
        var e = _arg2;
        Logger__logf$("ERROR", "Parsing response failed: %O", [e]);
        return AsyncBuilder__Return$FSharpOption_1_Result_1_GeneratorData_FSharpOption_1_Result_1_GeneratorData_(Async__get_async$(), {Tag: 0.000000});
      }));
    }));
});
LanguageService__send$NavigateData_NavigateData_ = (function(id,req)
{
    return AsyncBuilder__Delay$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__TryWith$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), AsyncBuilder__Delay$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), (function(_unitVar)
      {
        return AsyncBuilder__Bind$String____FSharpOption_1_Result_1_NavigateData_String____FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), req, (function(_arg1)
        {
          var r = _arg1;
          return AsyncBuilder__Return$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), LanguageService__parseResponse$NavigateData_NavigateData_(r)[id]);
        }));
      })), (function(_arg2)
      {
        var e = _arg2;
        Logger__logf$("ERROR", "Parsing response failed: %O", [e]);
        return AsyncBuilder__Return$FSharpOption_1_Result_1_NavigateData_FSharpOption_1_Result_1_NavigateData_(Async__get_async$(), {Tag: 0.000000});
      }));
    }));
});
LanguageService__send$Unit_Unit_ = (function(id,req)
{
    return AsyncBuilder__Delay$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__TryWith$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Async__get_async$(), AsyncBuilder__Delay$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Async__get_async$(), (function(_unitVar)
      {
        return AsyncBuilder__Bind$String____FSharpOption_1_Result_1_Unit_String____FSharpOption_1_Result_1_Unit_(Async__get_async$(), req, (function(_arg1)
        {
          var r = _arg1;
          return AsyncBuilder__Return$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Async__get_async$(), LanguageService__parseResponse$Unit_Unit_(r)[id]);
        }));
      })), (function(_arg2)
      {
        var e = _arg2;
        Logger__logf$("ERROR", "Parsing response failed: %O", [e]);
        return AsyncBuilder__Return$FSharpOption_1_Result_1_Unit_FSharpOption_1_Result_1_Unit_(Async__get_async$(), {Tag: 0.000000});
      }));
    }));
});
LanguageService__start$ = (function(unitVar0)
{
    try
    {
      var location = TextOnProcess__textonPath$();
      if ((location == null)) 
      {
        ;
      }
      else
      {
        var child = TextOnProcess__spawn$(location, TextOnProcess__fromPath$("mono"), ("--port " + LanguageService__port));
        LanguageService__service = {Tag: 1.000000, Value: child};
        var ignored0 = ((child.stderr).on("data", (function(n)
        {
          return ((window.console).error(n.toString()));
        })));
      };
    }
    catch(exc){
      ((window.console).error(exc));
      LanguageService__service = {Tag: 0.000000};
      var opt = ({});
      (opt.detail) = "Language services could not be spawned";
      null;
      (opt.dismissable) = true;
      null;
      var _ignored0 = (((window.atom).notifications).addError("Critical error", opt));
    };
});
LanguageService__stop$ = (function(unitVar0)
{
    Option__Iterate$ChildProcess_ChildProcess_((function(n)
    {
      return (n.kill("SIGKILL"));
    }), LanguageService__service);
    LanguageService__service = {Tag: 0.000000};
});
LanguageService__tryParse$Result_1_Error___Result_1_Error___ = (function(s)
{
    try
    {
      return (function(arg0)
      {
        return {Tag: 1.000000, Value: arg0};
      })(((window.JSON).parse(s)));
    }
    catch(ex){
      return {Tag: 0.000000};
    };
});
LanguageService__tryParse$Result_1_GeneratorData_Result_1_GeneratorData_ = (function(s)
{
    try
    {
      return (function(arg0)
      {
        return {Tag: 1.000000, Value: arg0};
      })(((window.JSON).parse(s)));
    }
    catch(ex){
      return {Tag: 0.000000};
    };
});
LanguageService__tryParse$Result_1_NavigateData_Result_1_NavigateData_ = (function(s)
{
    try
    {
      return (function(arg0)
      {
        return {Tag: 1.000000, Value: arg0};
      })(((window.JSON).parse(s)));
    }
    catch(ex){
      return {Tag: 0.000000};
    };
});
LanguageService__tryParse$Result_1_Unit_Result_1_Unit_ = (function(s)
{
    try
    {
      return (function(arg0)
      {
        return {Tag: 1.000000, Value: arg0};
      })(((window.JSON).parse(s)));
    }
    catch(ex){
      return {Tag: 0.000000};
    };
});
LanguageService__updateGenerator$ = (function(unitVar0)
{
    return LanguageService__send$GeneratorData_GeneratorData_(0, LanguageService__request$UpdateGeneratorRequest_UpdateGeneratorRequest_(LanguageService__url$("updategenerator"), (new UpdateGeneratorRequest___ctor$(""))));
});
LanguageService__url$ = (function(s)
{
    var clo1 = String__PrintFormatToString$("http://localhost:%s/%s");
    return (function(arg10)
    {
      var clo2 = clo1(arg10);
      return (function(arg20)
      {
        return clo2(arg20);
      });
    })(LanguageService__port)(s);
});
LintResult___ctor$ = (function(type,text,filePath,range)
{
    var __this = this;
    __this.type = type;
    __this.text = text;
    __this.filePath = filePath;
    __this.range = range;
});
List__CreateCons$JQuery_JQuery_ = (function(x,xs)
{
    return (new list_1_JQuery__ConsJQuery_(x, xs));
});
List__CreateCons$Object___Object___ = (function(x,xs)
{
    return (new list_1_Object____ConsObject___(x, xs));
});
List__CreateCons$String___String___ = (function(x,xs)
{
    return (new list_1_String____ConsString___(x, xs));
});
List__CreateCons$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(x,xs)
{
    return (new list_1_Tuple_2_IPane__Object__ConsTuple_2_IPane__Object_(x, xs));
});
List__CreateCons$Tuple_2_String__String_Tuple_2_String__String_ = (function(x,xs)
{
    return (new list_1_Tuple_2_String__String__ConsTuple_2_String__String_(x, xs));
});
List__Empty$JQuery_JQuery_ = (function()
{
    return (new list_1_JQuery__NilJQuery_());
});
List__Empty$Object___Object___ = (function()
{
    return (new list_1_Object____NilObject___());
});
List__Empty$String___String___ = (function()
{
    return (new list_1_String____NilString___());
});
List__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function()
{
    return (new list_1_Tuple_2_IPane__Object__NilTuple_2_IPane__Object_());
});
List__Empty$Tuple_2_String__String_Tuple_2_String__String_ = (function()
{
    return (new list_1_Tuple_2_String__String__NilTuple_2_String__String_());
});
List__Fold$JQuery__JQuery_JQuery__JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexed$JQuery__JQuery_JQuery__JQuery_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$JQuery__list_1_JQuery_JQuery__list_1_JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexed$JQuery__list_1_JQuery_JQuery__list_1_JQuery_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$String_1_Int32_String_Int32 = (function(f,seed,xs)
{
    return List__FoldIndexed$String_1_Int32_String_Int32((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$String_1_JQuery_String_JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexed$String_1_JQuery_String_JQuery_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$String_1_list_1_String_String_list_1_String_ = (function(f,seed,xs)
{
    return List__FoldIndexed$String_1_list_1_String_String_list_1_String_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_ = (function(f,seed,xs)
{
    return List__FoldIndexed$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__Fold$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_ = (function(f,seed,xs)
{
    return List__FoldIndexed$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
List__FoldIndexed$JQuery__JQuery_JQuery__JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$JQuery__JQuery_JQuery__JQuery_(f, 0, seed, xs);
});
List__FoldIndexed$JQuery__list_1_JQuery_JQuery__list_1_JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$list_1_JQuery__JQuery_list_1_JQuery__JQuery_(f, 0, seed, xs);
});
List__FoldIndexed$String_1_Int32_String_Int32 = (function(f,seed,xs)
{
    return List__FoldIndexedAux$Int32__String_1Int32_String(f, 0, seed, xs);
});
List__FoldIndexed$String_1_JQuery_String_JQuery_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$JQuery__String_1JQuery__String(f, 0, seed, xs);
});
List__FoldIndexed$String_1_Unit_String_Unit_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$Unit__String_1Unit__String(f, 0, seed, xs);
});
List__FoldIndexed$String_1_list_1_String_String_list_1_String_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$list_1_String__String_1list_1_String__String(f, 0, seed, xs);
});
List__FoldIndexed$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(f, 0, seed, xs);
});
List__FoldIndexed$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_ = (function(f,seed,xs)
{
    return List__FoldIndexedAux$list_1_String__Tuple_2_String__String_list_1_String__Tuple_2_String__String_(f, 0, seed, xs);
});
List__FoldIndexedAux$Int32__String_1Int32_String = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$Int32__String_1Int32_String(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$JQuery__JQuery_JQuery__JQuery_ = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$JQuery__JQuery_JQuery__JQuery_(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$JQuery__String_1JQuery__String = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$JQuery__String_1JQuery__String(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$Unit__String_1Unit__String = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$Unit__String_1Unit__String(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$list_1_JQuery__JQuery_list_1_JQuery__JQuery_ = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$list_1_JQuery__JQuery_list_1_JQuery__JQuery_(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$list_1_String__String_1list_1_String__String = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$list_1_String__String_1list_1_String__String(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$list_1_String__Tuple_2_String__String_list_1_String__Tuple_2_String__String_ = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$list_1_String__Tuple_2_String__String_list_1_String__Tuple_2_String__String_(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__FoldIndexedAux$list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,i,acc,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return List__FoldIndexedAux$list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_list_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(f, (i + 1), f(i)(acc)(x), xs);
    }
    else
    {
      return acc;
    };
});
List__Head$Object___Object___ = (function(_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return x;
    }
    else
    {
      throw ("List was empty");
      return null;
    };
});
List__Head$String___String___ = (function(_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return x;
    }
    else
    {
      throw ("List was empty");
      return null;
    };
});
List__IterateIndexed$String_1String = (function(f,xs)
{
    var _1178;
    return List__FoldIndexed$String_1_Unit_String_Unit_((function(i)
    {
      return (function(unitVar1)
      {
        return (function(x)
        {
          return f(i)(x);
        });
      });
    }), _1178, xs);
});
List__Length$String_1String = (function(xs)
{
    return List__Fold$String_1_Int32_String_Int32((function(acc)
    {
      return (function(_arg1)
      {
        return (acc + 1);
      });
    }), 0, xs);
});
List__Map$Tuple_2_String__String__String_1Tuple_2_String__String__String = (function(f,xs)
{
    return List__Reverse$String_1String(List__Fold$Tuple_2_String__String__list_1_String_Tuple_2_String__String__list_1_String_((function(acc)
    {
      return (function(x)
      {
        return (new list_1_String__ConsString(f(x), acc));
      });
    }), (new list_1_String__NilString()), xs));
});
List__OfArray$String_1String = (function(xs)
{
    return Array__FoldBack$String_1_list_1_String_String_list_1_String_((function(x)
    {
      return (function(acc)
      {
        return (new list_1_String__ConsString(x, acc));
      });
    }), xs, (new list_1_String__NilString()));
});
List__Reverse$JQuery_JQuery_ = (function(xs)
{
    return List__Fold$JQuery__list_1_JQuery_JQuery__list_1_JQuery_((function(acc)
    {
      return (function(x)
      {
        return (new list_1_JQuery__ConsJQuery_(x, acc));
      });
    }), (new list_1_JQuery__NilJQuery_()), xs);
});
List__Reverse$String_1String = (function(xs)
{
    return List__Fold$String_1_list_1_String_String_list_1_String_((function(acc)
    {
      return (function(x)
      {
        return (new list_1_String__ConsString(x, acc));
      });
    }), (new list_1_String__NilString()), xs);
});
List__Reverse$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(xs)
{
    return List__Fold$Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__list_1_Tuple_2_IPane__Object_((function(acc)
    {
      return (function(x)
      {
        return (new list_1_Tuple_2_IPane__Object__ConsTuple_2_IPane__Object_(x, acc));
      });
    }), (new list_1_Tuple_2_IPane__Object__NilTuple_2_IPane__Object_()), xs);
});
List__Tail$Object___Object___ = (function(_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return xs;
    }
    else
    {
      throw ("List was empty");
      return null;
    };
});
List__Tail$String___String___ = (function(_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      return xs;
    }
    else
    {
      throw ("List was empty");
      return null;
    };
});
List__ToArray$String_1String = (function(xs)
{
    var size = List__Length$String_1String(xs);
    var ys = Array__ZeroCreate$String_1String(size);
    List__IterateIndexed$String_1String((function(i)
    {
      return (function(x)
      {
        ys[i] = x;
        return null;
      });
    }), xs);
    return ys;
});
List__TryPick$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,xs)
{
    return List__TryPickIndexed$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(_arg1)
    {
      return (function(x)
      {
        return f(x);
      });
    }), xs);
});
List__TryPickIndexed$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,xs)
{
    return List__TryPickIndexedAux$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(f, 0, xs);
});
List__TryPickIndexedAux$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,i,_arg1)
{
    if ((_arg1.Tag == 1.000000)) 
    {
      var xs = _arg1.Item2;
      var x = _arg1.Item1;
      var result = f(i)(x);
      if ((result.Tag == 0.000000)) 
      {
        return List__TryPickIndexedAux$Tuple_2_IPane__Object__Tuple_2_IPane__Object_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(f, (i + 1), xs);
      }
      else
      {
        return result;
      };
    }
    else
    {
      return {Tag: 0.000000};
    };
});
Logger__activate$ = (function(name)
{
    Logger__active = true;
    Logger__logPath = {Tag: 1.000000, Value: (((window.atom).project).getPaths())[0]};
    return Logger__subscriptions.push(Control__CommandRegistry_subscribe$(((window.atom).commands), "atom-workspace", (("Debug:Show " + name) + " Log"), (function(_arg2)
    {
      return ((((window.atom).workspace).open("Debug log", (new OpenEditorOptions___ctor$("right")))).done((function(ed)
      {
        Logger__editor = {Tag: 1.000000, Value: ed};
        var ignored0 = (ed.onDidDestroy((function(_arg1)
        {
          Logger__editor = {Tag: 0.000000};
        })));
        var _ignored0 = (ed.insertText(Logger__fullLog));
        var view = (((window.atom).views).getView(ed));
        return view.component.setInputEnabled(false);
      })));
    })));
});
Logger__deactivate$ = (function(unitVar0)
{
    Logger__active = false;
    Logger__logPath = {Tag: 0.000000};
    Logger__editor = {Tag: 0.000000};
    Seq__Iterate$Disposable_Disposable_((function(s)
    {
      var ignored0 = (s.dispose());
    }), ResizeArray__ToSeq$Disposable_Disposable_(Logger__subscriptions));
    return (Logger__subscriptions = []);
});
Logger__emitLog$ = (function(category,message,data)
{
    var msg = ((("[" + category.toUpperCase()) + "] ") + message);
    if ((Array__BoxedLength$(data) == 0)) 
    {
      ((window.console).log(msg));
    }
    else
    {
      console.log.apply(console, Array__Append$Object_Object_([msg], data));
    };
    var copyOfStruct = DateTime__get_Now$();
    var timeString = String__Replace$(String__Replace$(DateTime__ToLongTimeString$(copyOfStruct), "\\", "."), "/", ".");
    var _msg = ((((("[" + timeString) + "] ") + category.toUpperCase()) + "\n  ") + String__Replace$(message, "%O", "%s"));
    var logLine = (util.format.apply(util, Array__Append$Object_Object_([_msg], data)) + "\n");
    Logger__fullLog = (Logger__fullLog + logLine);
    Option__Iterate$String_1String((function(p)
    {
      return (fs.appendFile((p + "/.texton.debug"), logLine));
    }), Logger__logPath);
    return Option__Iterate$IEditor_IEditor_((function(e)
    {
      var ignored0 = (e.scrollToBufferPosition([(e.getLastBufferRow()), 0.000000], null));
      var _ignored0 = ((e.getBuffer()).append(logLine));
    }), Logger__editor);
});
Logger__get_active$ = (function()
{
    return false;
});
Logger__get_editor$ = (function()
{
    return {Tag: 0.000000};
});
Logger__get_fullLog$ = (function()
{
    return "";
});
Logger__get_logPath$ = (function()
{
    return {Tag: 0.000000};
});
Logger__get_subscriptions$ = (function()
{
    return [];
});
Logger__logf$ = (function(category,format,o)
{
    var debug = (((window.atom).config).get("texton.DeveloperMode"));
    if ((debug && Logger__active)) 
    {
      return Logger__emitLog$(category, format, o);
    }
    else
    {
      ;
    };
});
NavigateRequest___ctor$ = (function(FileName,NavigateType,Name)
{
    var __this = this;
    __this.FileName = FileName;
    __this.NavigateType = NavigateType;
    __this.Name = Name;
});
OpenEditorOptions___ctor$ = (function(split)
{
    var __this = this;
    __this.split = split;
});
OpenOptions___ctor$ = (function(initialLine,initialColumn)
{
    var __this = this;
    __this.initialLine = initialLine;
    __this.initialColumn = initialColumn;
});
Option__GetValue$Boolean_Boolean = (function(option)
{
    return option.Value;;
});
Option__GetValue$CancellationToken_CancellationToken_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$ChildProcess_ChildProcess_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Disposable_Disposable_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$FSharpList_1_Object___FSharpList_1_Object___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$FSharpList_1_String___FSharpList_1_String___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$FSharpRef_1_Boolean_FSharpRef_1_Boolean_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$IEditor_IEditor_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$IEnumerator_1_IPane_IEnumerator_1_IPane_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$IEnumerator_1_Object_IEnumerator_1_Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$IEnumerator_1_Tuple_2_IPane__Object_IEnumerator_1_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Int32_Int32 = (function(option)
{
    return option.Value;;
});
Option__GetValue$Result_1_Error___Result_1_Error___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Result_1_LintWarning___Result_1_LintWarning___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Result_1_NavigateData_Result_1_NavigateData_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Result_1_Unit_Result_1_Unit_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$String_1String = (function(option)
{
    return option.Value;;
});
Option__GetValue$Timer_Timer_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Disposable__Int32_Tuple_2_Disposable__Int32_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_IPane__Int32_Tuple_2_IPane__Int32_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Object__Int32_Tuple_2_Object__Int32_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Object____FSharpList_1_Object___Tuple_2_Object____FSharpList_1_Object___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_String____FSharpList_1_String___Tuple_2_String____FSharpList_1_String___ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_String____Int32_Tuple_2_String____Int32_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Tuple_2_IPane__Object__Boolean_Tuple_2_Tuple_2_IPane__Object__Boolean_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_ = (function(option)
{
    return option.Value;;
});
Option__IsSome$Boolean_Boolean = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$FSharpList_1_Object___FSharpList_1_Object___ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$FSharpList_1_String___FSharpList_1_String___ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$IEnumerator_1_IPane_IEnumerator_1_IPane_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$IEnumerator_1_Object_IEnumerator_1_Object_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$Int32_Int32 = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$Result_1_NavigateData_Result_1_NavigateData_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__IsSome$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(option)
{
    return ((option.Tag == 1.000000) && true);
});
Option__Iterate$ChildProcess_ChildProcess_ = (function(f,inp)
{
    if ((inp.Tag == 1.000000)) 
    {
      var x = Option__GetValue$ChildProcess_ChildProcess_(inp);
      return f(x);
    }
    else
    {
      ;
    };
});
Option__Iterate$Disposable_Disposable_ = (function(f,inp)
{
    if ((inp.Tag == 1.000000)) 
    {
      var x = Option__GetValue$Disposable_Disposable_(inp);
      return f(x);
    }
    else
    {
      ;
    };
});
Option__Iterate$IEditor_IEditor_ = (function(f,inp)
{
    if ((inp.Tag == 1.000000)) 
    {
      var x = Option__GetValue$IEditor_IEditor_(inp);
      return f(x);
    }
    else
    {
      ;
    };
});
Option__Iterate$String_1String = (function(f,inp)
{
    if ((inp.Tag == 1.000000)) 
    {
      var x = Option__GetValue$String_1String(inp);
      return f(x);
    }
    else
    {
      ;
    };
});
Option__Iterate$Timer_Timer_ = (function(f,inp)
{
    if ((inp.Tag == 1.000000)) 
    {
      var x = Option__GetValue$Timer_Timer_(inp);
      return f(x);
    }
    else
    {
      ;
    };
});
Options___ctor$ = (function(cwd)
{
    var __this = this;
    __this.cwd = cwd;
});
ParseRequest___ctor$ = (function(FileName,IsAsync,Lines)
{
    var __this = this;
    __this.FileName = FileName;
    __this.IsAsync = IsAsync;
    __this.Lines = Lines;
});
Provider___ctor$ = (function(grammarScopes,scope,lintOnFly,lint)
{
    var __this = this;
    __this.grammarScopes = grammarScopes;
    __this.scope = scope;
    __this.lintOnFly = lintOnFly;
    __this.lint = lint;
});
Replacements__utf8Encoding$ = (function(unitVar0)
{
    return (new UTF8Encoding___ctor$());
});
ResizeArray_1_Object__get_Count$Object_ = (function(xs,unitVar1)
{
    return xs.length;
});
ResizeArray_1_Object__get_Item$Object_ = (function(xs,index)
{
    return xs[index];
});
ResizeArray__ToSeq$Disposable_Disposable_ = (function(xs)
{
    return Seq__Unfold$Int32__Disposable_Int32_Disposable_((function(i)
    {
      if ((i < ResizeArray_1_Object__get_Count$Object_(xs))) 
      {
        return {Tag: 1.000000, Value: (new TupleDisposable__Int32(ResizeArray_1_Object__get_Item$Object_(xs, i), (i + 1)))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), 0);
});
Seq__Collect$IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,xs)
{
    return Seq__Concat$IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(Seq__Map$IPane__IEnumerable_1_Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object_(f, xs));
});
Seq__Collect$Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,xs)
{
    return Seq__Concat$IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(Seq__Map$Object__IEnumerable_1_Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object_(f, xs));
});
Seq__Concat$IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(xs)
{
    return Seq__Delay$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var _enum = Seq__Enumerator$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_(xs);
      var tryGetNext = (function(innerEnum)
      {
        var _innerEnum = {contents: innerEnum};
        var output = {contents: {Tag: 0.000000}};
        var hasFinished = {contents: false};
        while ((!hasFinished.contents))
        {
          var matchValue = _innerEnum.contents;
          if ((matchValue.Tag == 1.000000)) 
          {
            var currentEnum = Option__GetValue$IEnumerator_1_Tuple_2_IPane__Object_IEnumerator_1_Tuple_2_IPane__Object_(matchValue);
            if (currentEnum.MoveNext()) 
            {
              output.contents = {Tag: 1.000000, Value: currentEnum.get_Current()};
              null;
              hasFinished.contents = true;
              null;
            }
            else
            {
              _innerEnum.contents = {Tag: 0.000000};
              null;
            };
          }
          else
          {
            if (_enum.MoveNext()) 
            {
              _innerEnum.contents = {Tag: 1.000000, Value: Seq__Enumerator$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(_enum.get_Current())};
              null;
            }
            else
            {
              hasFinished.contents = true;
              null;
            };
          };
        };
        var _matchValue = (new TupleFSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_(_innerEnum.contents, output.contents));
        if ((_matchValue.Items[0.000000].Tag == 1.000000)) 
        {
          if ((_matchValue.Items[1.000000].Tag == 1.000000)) 
          {
            var e = Option__GetValue$IEnumerator_1_Tuple_2_IPane__Object_IEnumerator_1_Tuple_2_IPane__Object_(_matchValue.Items[0.000000]);
            var x = Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(_matchValue.Items[1.000000]);
            return {Tag: 1.000000, Value: (new TupleTuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_(x, {Tag: 1.000000, Value: e}))};
          }
          else
          {
            return {Tag: 0.000000};
          };
        }
        else
        {
          return {Tag: 0.000000};
        };
      });
      return Seq__Unfold$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(x)
      {
        return tryGetNext(x);
      }), {Tag: 0.000000});
    }));
});
Seq__Delay$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_ = (function(f)
{
    return Seq__FromFactory$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var _2171;
      return Seq__Enumerator$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_(f(_2171));
    }));
});
Seq__Delay$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(f)
{
    return Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var _2318;
      return Seq__Enumerator$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(f(_2318));
    }));
});
Seq__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function()
{
    return Seq__Unfold$Boolean__Tuple_2_IPane__Object_Boolean_Tuple_2_IPane__Object_((function(_arg1)
    {
      return {Tag: 0.000000};
    }), false);
});
Seq__Enumerator$Disposable_Disposable_ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$IPane_IPane_ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$Object_Object_ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$Object___Object___ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$String___String___ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Enumerator$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(xs)
{
    return xs.GetEnumerator();
});
Seq__Fold$Disposable__Unit_Disposable__Unit_ = (function(f,seed,xs)
{
    return Seq__FoldIndexed$Disposable__Unit_Disposable__Unit_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Seq__Fold$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_ = (function(f,seed,xs)
{
    return Seq__FoldIndexed$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_((function(_arg1)
    {
      return (function(acc)
      {
        return (function(x)
        {
          return f(acc)(x);
        });
      });
    }), seed, xs);
});
Seq__FoldIndexed$Disposable__Unit_Disposable__Unit_ = (function(f,seed,xs)
{
    return Seq__FoldIndexedAux$Unit__Disposable_Unit__Disposable_(f, seed, Seq__Enumerator$Disposable_Disposable_(xs));
});
Seq__FoldIndexed$Object____Unit_Object____Unit_ = (function(f,seed,xs)
{
    return Seq__FoldIndexedAux$Unit__Object___Unit__Object___(f, seed, Seq__Enumerator$Object___Object___(xs));
});
Seq__FoldIndexed$String____Unit_String____Unit_ = (function(f,seed,xs)
{
    return Seq__FoldIndexedAux$Unit__String___Unit__String___(f, seed, Seq__Enumerator$String___String___(xs));
});
Seq__FoldIndexed$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_ = (function(f,seed,xs)
{
    return Seq__FoldIndexedAux$FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(f, seed, Seq__Enumerator$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(xs));
});
Seq__FoldIndexedAux$FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpList_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,acc,xs)
{
    var i = {contents: 0};
    var _acc = {contents: acc};
    while (xs.MoveNext())
    {
      _acc.contents = f(i.contents)(_acc.contents)(xs.get_Current());
      null;
      i.contents = (i.contents + 1);
      null;
    };
    return _acc.contents;
});
Seq__FoldIndexedAux$Unit__Disposable_Unit__Disposable_ = (function(f,acc,xs)
{
    var i = {contents: 0};
    var _acc = {contents: acc};
    while (xs.MoveNext())
    {
      _acc.contents = f(i.contents)(_acc.contents)(xs.get_Current());
      null;
      i.contents = (i.contents + 1);
      null;
    };
    return _acc.contents;
});
Seq__FoldIndexedAux$Unit__Object___Unit__Object___ = (function(f,acc,xs)
{
    var i = {contents: 0};
    var _acc = {contents: acc};
    while (xs.MoveNext())
    {
      _acc.contents = f(i.contents)(_acc.contents)(xs.get_Current());
      null;
      i.contents = (i.contents + 1);
      null;
    };
    return _acc.contents;
});
Seq__FoldIndexedAux$Unit__String___Unit__String___ = (function(f,acc,xs)
{
    var i = {contents: 0};
    var _acc = {contents: acc};
    while (xs.MoveNext())
    {
      _acc.contents = f(i.contents)(_acc.contents)(xs.get_Current());
      null;
      i.contents = (i.contents + 1);
      null;
    };
    return _acc.contents;
});
Seq__FromFactory$Disposable_Disposable_ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_Disposable___ctor$Disposable_(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _101;
        return __.factory(_101);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerable_1_Tuple_2_IPane__Object_(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _2163;
        return __.factory(_2163);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$IPane_IPane_ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_IPane___ctor$IPane_(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _2393;
        return __.factory(_2393);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$Object_Object_ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_Object___ctor$Object_(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _2083;
        return __.factory(_2083);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$Object___Object___ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_Object_____ctor$Object___(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _6024;
        return __.factory(_6024);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$String___String___ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_String_____ctor$String___(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _455;
        return __.factory(_455);
      })(impl, unitVar1);
    })};
});
Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(f)
{
    var impl;
    impl = (new CreateEnumerable_1_Tuple_2_IPane__Object___ctor$Tuple_2_IPane__Object_(f));
    return {GetEnumerator: (function(unitVar1)
    {
      return (function(__,unitVar1)
      {
        var _1944;
        return __.factory(_1944);
      })(impl, unitVar1);
    })};
});
Seq__Iterate$Disposable_Disposable_ = (function(f,xs)
{
    var _109;
    return Seq__Fold$Disposable__Unit_Disposable__Unit_((function(unitVar0)
    {
      return (function(x)
      {
        return f(x);
      });
    }), _109, xs);
});
Seq__IterateIndexed$Object___Object___ = (function(f,xs)
{
    var _6044;
    return Seq__FoldIndexed$Object____Unit_Object____Unit_((function(i)
    {
      return (function(unitVar1)
      {
        return (function(x)
        {
          return f(i)(x);
        });
      });
    }), _6044, xs);
});
Seq__IterateIndexed$String___String___ = (function(f,xs)
{
    var _475;
    return Seq__FoldIndexed$String____Unit_String____Unit_((function(i)
    {
      return (function(unitVar1)
      {
        return (function(x)
        {
          return f(i)(x);
        });
      });
    }), _475, xs);
});
Seq__Map$IPane__IEnumerable_1_Tuple_2_IPane__Object_IPane__IEnumerable_1_Tuple_2_IPane__Object_ = (function(f,xs)
{
    return Seq__Delay$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_((function(unitVar0)
    {
      return Seq__Unfold$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_((function(_enum)
      {
        if (_enum.MoveNext()) 
        {
          return {Tag: 1.000000, Value: (new TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_(f(_enum.get_Current()), _enum))};
        }
        else
        {
          return {Tag: 0.000000};
        };
      }), Seq__Enumerator$IPane_IPane_(xs));
    }));
});
Seq__Map$Object__IEnumerable_1_Tuple_2_IPane__Object_Object__IEnumerable_1_Tuple_2_IPane__Object_ = (function(f,xs)
{
    return Seq__Delay$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_((function(unitVar0)
    {
      return Seq__Unfold$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_((function(_enum)
      {
        if (_enum.MoveNext()) 
        {
          return {Tag: 1.000000, Value: (new TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_(f(_enum.get_Current()), _enum))};
        }
        else
        {
          return {Tag: 0.000000};
        };
      }), Seq__Enumerator$Object_Object_(xs));
    }));
});
Seq__OfArray$IPane_IPane_ = (function(xs)
{
    return Seq__Unfold$Int32__IPane_Int32_IPane_((function(i)
    {
      if ((i < Array__BoxedLength$(xs))) 
      {
        return {Tag: 1.000000, Value: (new TupleIPane__Int32(xs[i], (i + 1)))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), 0);
});
Seq__OfArray$Object_Object_ = (function(xs)
{
    return Seq__Unfold$Int32__Object_Int32_Object_((function(i)
    {
      if ((i < Array__BoxedLength$(xs))) 
      {
        return {Tag: 1.000000, Value: (new TupleObject__Int32(xs[i], (i + 1)))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), 0);
});
Seq__OfArray$String___String___ = (function(xs)
{
    return Seq__Unfold$Int32__String___Int32_String___((function(i)
    {
      if ((i < Array__BoxedLength$(xs))) 
      {
        return {Tag: 1.000000, Value: (new TupleString____Int32(xs[i], (i + 1)))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), 0);
});
Seq__OfList$Object___Object___ = (function(xs)
{
    return Seq__Unfold$FSharpList_1_Object____Object___FSharpList_1_Object____Object___((function(_arg1)
    {
      if ((_arg1.Tag == 1.000000)) 
      {
        var _xs = List__Tail$Object___Object___(_arg1);
        var x = List__Head$Object___Object___(_arg1);
        return {Tag: 1.000000, Value: (new TupleObject____FSharpList_1_Object___(x, _xs))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), xs);
});
Seq__OfList$String___String___ = (function(xs)
{
    return Seq__Unfold$FSharpList_1_String____String___FSharpList_1_String____String___((function(_arg1)
    {
      if ((_arg1.Tag == 1.000000)) 
      {
        var _xs = List__Tail$String___String___(_arg1);
        var x = List__Head$String___String___(_arg1);
        return {Tag: 1.000000, Value: (new TupleString____FSharpList_1_String___(x, _xs))};
      }
      else
      {
        return {Tag: 0.000000};
      };
    }), xs);
});
Seq__Singleton$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(x)
{
    return Seq__Unfold$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_((function(_arg1)
    {
      if ((_arg1.Tag == 0.000000)) 
      {
        return {Tag: 0.000000};
      }
      else
      {
        var _x = Option__GetValue$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(_arg1);
        return {Tag: 1.000000, Value: (new TupleTuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_(_x, {Tag: 0.000000}))};
      };
    }), {Tag: 1.000000, Value: x});
});
Seq__ToArray$Object___Object___ = (function(xs)
{
    var ys = Array__ZeroCreate$Object___Object___(0);
    Seq__IterateIndexed$Object___Object___((function(i)
    {
      return (function(x)
      {
        ys[i] = x;
        return null;
      });
    }), xs);
    return ys;
});
Seq__ToArray$String___String___ = (function(xs)
{
    var ys = Array__ZeroCreate$String___String___(0);
    Seq__IterateIndexed$String___String___((function(i)
    {
      return (function(x)
      {
        ys[i] = x;
        return null;
      });
    }), xs);
    return ys;
});
Seq__ToList$Tuple_2_IPane__Object_Tuple_2_IPane__Object_ = (function(xs)
{
    return List__Reverse$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(Seq__Fold$Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_Tuple_2_IPane__Object__FSharpList_1_Tuple_2_IPane__Object_((function(acc)
    {
      return (function(x)
      {
        return List__CreateCons$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(x, acc);
      });
    }), List__Empty$Tuple_2_IPane__Object_Tuple_2_IPane__Object_(), xs));
});
Seq__Unfold$Boolean__Tuple_2_IPane__Object_Boolean_Tuple_2_IPane__Object_ = (function(f,seed)
{
    return Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_Boolean__Tuple_2_IPane__Object___ctor$Boolean_Tuple_2_IPane__Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$Boolean_Boolean(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__Boolean_Tuple_2_Tuple_2_IPane__Object__Boolean_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__Boolean_Tuple_2_Tuple_2_IPane__Object__Boolean_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$Boolean_Boolean(__.acc) && (function()
          {
            var _1988;
            return next(_1988);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$FSharpList_1_Object____Object___FSharpList_1_Object____Object___ = (function(f,seed)
{
    return Seq__FromFactory$Object___Object___((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_FSharpList_1_Object____Object_____ctor$FSharpList_1_Object____Object___(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$FSharpList_1_Object___FSharpList_1_Object___(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Object____FSharpList_1_Object___Tuple_2_Object____FSharpList_1_Object___(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Object____FSharpList_1_Object___Tuple_2_Object____FSharpList_1_Object___(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$FSharpList_1_Object___FSharpList_1_Object___(__.acc) && (function()
          {
            var _6002;
            return next(_6002);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$FSharpList_1_String____String___FSharpList_1_String____String___ = (function(f,seed)
{
    return Seq__FromFactory$String___String___((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_FSharpList_1_String____String_____ctor$FSharpList_1_String____String___(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$FSharpList_1_String___FSharpList_1_String___(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_String____FSharpList_1_String___Tuple_2_String____FSharpList_1_String___(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_String____FSharpList_1_String___Tuple_2_String____FSharpList_1_String___(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$FSharpList_1_String___FSharpList_1_String___(__.acc) && (function()
          {
            var _659;
            return next(_659);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,seed)
{
    return Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_(__.acc) && (function()
          {
            var _2295;
            return next(_2295);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(f,seed)
{
    return Seq__FromFactory$Tuple_2_IPane__Object_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_Tuple_2_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$FSharpOption_1_Tuple_2_IPane__Object_FSharpOption_1_Tuple_2_IPane__Object_(__.acc) && (function()
          {
            var _1922;
            return next(_1922);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_ = (function(f,seed)
{
    return Seq__FromFactory$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$IEnumerator_1_IPane_IEnumerator_1_IPane_(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$IEnumerator_1_IPane_IEnumerator_1_IPane_(__.acc) && (function()
          {
            var _2451;
            return next(_2451);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_ = (function(f,seed)
{
    return Seq__FromFactory$IEnumerable_1_Tuple_2_IPane__Object_IEnumerable_1_Tuple_2_IPane__Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$IEnumerator_1_Object_IEnumerator_1_Object_(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_Tuple_2_IEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$IEnumerator_1_Object_IEnumerator_1_Object_(__.acc) && (function()
          {
            var _2141;
            return next(_2141);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$Int32__Disposable_Int32_Disposable_ = (function(f,seed)
{
    return Seq__FromFactory$Disposable_Disposable_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_Int32__Disposable___ctor$Int32_Disposable_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$Int32_Int32(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Disposable__Int32_Tuple_2_Disposable__Int32_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Disposable__Int32_Tuple_2_Disposable__Int32_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$Int32_Int32(__.acc) && (function()
          {
            var _79;
            return next(_79);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$Int32__IPane_Int32_IPane_ = (function(f,seed)
{
    return Seq__FromFactory$IPane_IPane_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_Int32__IPane___ctor$Int32_IPane_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$Int32_Int32(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_IPane__Int32_Tuple_2_IPane__Int32_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_IPane__Int32_Tuple_2_IPane__Int32_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$Int32_Int32(__.acc) && (function()
          {
            var _2371;
            return next(_2371);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$Int32__Object_Int32_Object_ = (function(f,seed)
{
    return Seq__FromFactory$Object_Object_((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_Int32__Object___ctor$Int32_Object_(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$Int32_Int32(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_Object__Int32_Tuple_2_Object__Int32_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_Object__Int32_Tuple_2_Object__Int32_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$Int32_Int32(__.acc) && (function()
          {
            var _2061;
            return next(_2061);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Seq__Unfold$Int32__String___Int32_String___ = (function(f,seed)
{
    return Seq__FromFactory$String___String___((function(unitVar0)
    {
      var impl;
      impl = (new UnfoldEnumerator_2_Int32__String_____ctor$Int32_String___(seed, f));
      return {get_Current: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          return __.current;
        })(impl, unitVar1);
      }), Dispose: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          ;
        })(impl, unitVar1);
      }), MoveNext: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          var next = (function(_unitVar0)
          {
            var currAcc = Option__GetValue$Int32_Int32(__.acc);
            var x = __.unfold(currAcc);
            if ((x.Tag == 1.000000)) 
            {
              var value = Option__GetValue$Tuple_2_String____Int32_Tuple_2_String____Int32_(x).Items[0.000000];
              var nextAcc = Option__GetValue$Tuple_2_String____Int32_Tuple_2_String____Int32_(x).Items[1.000000];
              __.acc = {Tag: 1.000000, Value: nextAcc};
              __.current = value;
              return true;
            }
            else
            {
              __.acc = {Tag: 0.000000};
              __.current = null;
              return false;
            };
          });
          return (Option__IsSome$Int32_Int32(__.acc) && (function()
          {
            var _433;
            return next(_433);
          })());
        })(impl, unitVar1);
      }), Reset: (function(unitVar1)
      {
        return (function(__,unitVar1)
        {
          __.acc = {Tag: 1.000000, Value: __.seed};
          __.current = null;
        })(impl, unitVar1);
      })};
    }));
});
Stream__Write$ = (function(__,buffer,offset,count)
{
    var extra = Array__GetSubArray$Byte_Byte(buffer, offset, count);
    __.contents = Array__Append$Byte_Byte(__.contents, extra);
});
Stream___ctor$ = (function(initalContents,flush)
{
    var __this = this;
    {};
    __this.flush = flush;
    __this.contents = initalContents;
    __this.nextIndex = 0;
});
Stream__get_Contents$ = (function(__,unitVar1)
{
    return __.contents;
});
String__EndsWith$ = (function(s,search)
{
    var offset = (s.length - search.length);
    var index = s.indexOf(search, offset);
    return ((index != -1) && (index == offset));
});
String__PrintFormatToString$ = (function(s)
{
    var reg = /%[+\-* ]?\d*(?:\.(\d+))?(\w)/;
    function formatToString(rep) {
        s = s.replace(reg, function(match, precision, format) {
            switch (format) {
                case "f": case "F": return precision ? rep.toFixed(precision) : rep.toFixed(6);
                case "g": case "G": return rep.toPrecision(precision);
                case "e": case "E": return rep.toExponential(precision);
                case "A": return JSON.stringify(rep);
                default:  return rep;
            }
        });
        return reg.test(s) ? formatToString : s;
    }
    return formatToString;
});
String__Replace$ = (function(s,search,replace)
{
    var splits = s.split(search);
    return splits.join(replace);
});
String__SplitWithoutOptions$ = (function(s,delimiters)
{
    var folder = (function(inputs)
    {
      return (function(delimiter)
      {
        return Array__Concat$String_1String(Seq__OfArray$String___String___(Array__Map$String_1_String___String_String___((function(inp)
        {
          return inp.split(delimiter);
        }), inputs)));
      });
    });
    var state = [s];
    return (function(array)
    {
      return Array__Fold$String_1_String___String_String___(folder, state, array);
    })(delimiters);
});
String__StartsWith$ = (function(s,search)
{
    return (s.indexOf(search) == 0);
});
TextOnCommands__openSettings$ = (function(unitVar0)
{
    var _5271;
    return (((window.atom).workspace).open("atom://config/packages/texton", _5271));
});
TextOnGenerator___ctor$ = (function(unitVar0)
{
    var __this = this;
    {};
    __this.disp = {Tag: 0.000000};
    __this.sub = {Tag: 0.000000};
});
TextOnGenerator__activate$ = (function(x,state)
{
    Logger__activate$("TextOnGenerator");
    var ignored0 = (((window.atom).commands).add("atom-workspace", "TextOn:Open-Generator", (function(unitVar0)
    {
      return Async_1_StartImmediate$(GeneratorPane__openTextOnGeneratorPane$(false), {Tag: 0.000000});
    })));
    (((window.atom).workspace).addOpener((function(uri)
    {
      try
      {
        if (String__EndsWith$(uri, "TextOn Generator")) 
        {
          return new GeneratorPane();
        }
        else
        {
          return null;
        };
      }
      catch(matchValue){
        return null;
      };
    })));
    var _ignored0 = (((window.atom).commands).add("atom-text-editor", "TextOn:Send-To-Generator", (function(unitVar0)
    {
      return Async_1_StartImmediate$(GeneratorPane__sendToTextOnGenerator$(false), {Tag: 0.000000});
    })));
    x.disp = {Tag: 1.000000, Value: (((window.atom).workspace).onDidChangeActivePaneItem((function(unitVar0)
    {
      Option__Iterate$Disposable_Disposable_((function(d)
      {
        return (d.dispose());
      }), x.sub);
      x.sub = {Tag: 0.000000};
      var e = (((window.atom).workspace).getActiveTextEditor());
      if (TextOnViewHelpers__isTextOnEditor$(e)) 
      {
        x.sub = {Tag: 1.000000, Value: TextOnViewHelpers__OnCursorStopMoving$Object_Object_(e, 100.000000, (function(_arg1)
        {
          var _5077;
          var arg10_ = _5077;
          return TextOnGenerator__updateGenerator$(x);
        }))};
      }
      else
      {
        ;
      };
    })))};
});
TextOnGenerator__deactivate$ = (function(x,unitVar1)
{
    Logger__deactivate$();
    Option__Iterate$Disposable_Disposable_((function(d)
    {
      return (d.dispose());
    }), x.disp);
    x.disp = {Tag: 0.000000};
    Option__Iterate$Disposable_Disposable_((function(d)
    {
      return (d.dispose());
    }), x.sub);
    x.sub = {Tag: 0.000000};
});
TextOnGenerator__updateGenerator$ = (function(_this,unitVar0)
{
    var ignored0 = Async_1_StartImmediate$(AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(unitVar)
    {
      return AsyncBuilder__Bind$FSharpOption_1_Result_1_GeneratorData__Unit_FSharpOption_1_Result_1_GeneratorData__Unit_(Async__get_async$(), LanguageService__updateGenerator$(), (function(_arg1)
      {
        var generatorData = _arg1;
        var _5166;
        if (Option__IsSome$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData)) 
        {
          var data = Option__GetValue$Result_1_GeneratorData_Result_1_GeneratorData_(generatorData).Data;
          _5166 = AsyncBuilder__Bind$Unit__Unit_Unit__Unit_(Async__get_async$(), GeneratorPane__replaceTextOnGeneratorHtmlPanel$(data), (function(_arg2)
          {
            var _5187;
            return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _5187);
          }));
        }
        else
        {
          _5166 = AsyncBuilder__Zero$(Async__get_async$());
        };
        return AsyncBuilder__Combine$Unit_Unit_(Async__get_async$(), _5166, AsyncBuilder__Delay$Unit_Unit_(Async__get_async$(), (function(_unitVar)
        {
          var _5199;
          return AsyncBuilder__Return$Unit_Unit_(Async__get_async$(), _5199);
        })));
      }));
    })), {Tag: 0.000000});
});
TextOnIDE___ctor$ = (function(unitVar0)
{
    var __this = this;
    {};
    __this.subscriptions = [];
    __this.generator = (new TextOnGenerator___ctor$());
});
TextOnIDE__activate$ = (function(x,state)
{
    var debug = (((window.atom).config).get("texton.DeveloperMode"));
    if (debug) 
    {
      Logger__activate$("TextOn IDE");
    }
    else
    {
      ;
    };
    LanguageService__start$();
    var _710;
    TextOnGenerator__activate$(x.generator, _710);
    x.subscriptions.push(Control__IConfig_onDidChange$ConfigChange_1_Boolean_ConfigChange_1_Boolean_(((window.atom).config), "texton.DeveloperMode", (function(n)
    {
      if (n.newValue) 
      {
        return Logger__activate$("TextOn IDE");
      }
      else
      {
        return Logger__deactivate$();
      };
    })));
    var ignored0 = (((window.atom).commands).add("atom-workspace", "TextOn:Settings", (function(arg00_)
    {
      return TextOnCommands__openSettings$();
    })));
});
TextOnIDE__deactivate$ = (function(x,unitVar1)
{
    Seq__Iterate$Disposable_Disposable_((function(n)
    {
      return (n.dispose());
    }), ResizeArray__ToSeq$Disposable_Disposable_(x.subscriptions));
    (x.subscriptions = []);
    TextOnGenerator__deactivate$(x.generator);
    LanguageService__stop$();
    return Logger__deactivate$();
});
TextOnIDE__provideErrors$ = (function(__,unitVar1)
{
    return ErrorLinterProvider__create$();
});
TextOnProcess__fromPath$ = (function(name)
{
    if (TextOnProcess__isWin$()) 
    {
      return name;
    }
    else
    {
      var path = (((window.atom).config).get("texton.MonoPath"));
      return ((path + "/") + name);
    };
});
TextOnProcess__getCwd$ = (function(unitVar0)
{
    try
    {
      var t = (((window.atom).project).getPaths())[0];
      if ((fs.existsSync(t))) 
      {
        return t;
      }
      else
      {
        return null;
      };
    }
    catch(matchValue){
      return null;
    };
});
TextOnProcess__isWin$ = (function(unitVar0)
{
    return String__StartsWith$(((window.process).platform), "win");
});
TextOnProcess__spawn$ = (function(location,linuxCmd,cmd)
{
    var _343;
    if ((cmd == "")) 
    {
      _343 = [];
    }
    else
    {
      _343 = String__SplitWithoutOptions$(cmd, [" "]);
    };
    var cmd_ = _343;
    var cwd = TextOnProcess__getCwd$();
    var _566;
    try
    {
      _566 = (new Options___ctor$(cwd));
    }
    catch(matchValue){
      _566 = (new Options___ctor$(null));
    };
    var options = _566;
    var _571;
    if (TextOnProcess__isWin$()) 
    {
      _571 = (child_process.spawn(location, cmd_, options));
    }
    else
    {
      var prms = Array__Concat$String_1String(Seq__OfList$String___String___(List__CreateCons$String___String___([location], List__CreateCons$String___String___(cmd_, List__Empty$String___String___()))));
      _571 = (child_process.spawn(linuxCmd, prms, options));
    };
    var procs = _571;
    return procs;
});
TextOnProcess__textonPath$ = (function(unitVar0)
{
    var path = (((window.atom).config).get("texton.TextOnPath"));
    if (((path == null) || (path == ""))) 
    {
      return null;
    }
    else
    {
      if (TextOnProcess__isWin$()) 
      {
        return (path + "\\TextOn.Atom.exe");
      }
      else
      {
        return (path + "/TextOn.Atom.exe");
      };
    };
});
TextOnViewHelpers__OnCursorStopMoving$Object_Object_ = (function(editor,timeout,callback)
{
    var timer = {contents: {Tag: 0.000000}};
    return (editor.onDidChangeCursorPosition((function(n)
    {
      return TextOnViewHelpers__stopMovingHandler$Object_Object_(timer, timeout, callback, n);
    })));
});
TextOnViewHelpers__clearTimer$Timer_Timer_ = (function(timer)
{
    Option__Iterate$Timer_Timer_((function(arg00)
    {
      return (window.clearTimeout(arg00));
    }), timer.contents);
    timer.contents = {Tag: 0.000000};
});
TextOnViewHelpers__isTextOnEditor$ = (function(editor)
{
    return ((((editor != undefined) && (editor["getGrammar"] != undefined)) && ((editor.getGrammar()) != undefined)) && (((editor.getGrammar()).name).indexOf("texton") >= 0));
});
TextOnViewHelpers__jq$ = (function(selector)
{
    return ((window.$)(selector));
});
TextOnViewHelpers__stopMovingHandler$Object_Object_ = (function(timer,timeout,callback,n)
{
    TextOnViewHelpers__clearTimer$Timer_Timer_(timer);
    timer.contents = {Tag: 1.000000, Value: (window.setTimeout((function(_arg1)
    {
      return callback(n);
    }), timeout))};
});
TimeSpan__get_TicksPerMillisecond$ = (function(unitVar0)
{
    return 10000.000000;
});
TupleDisposable__Int32 = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleFSharpFunc_2_Unit__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_ = (function(Item0,Item1,Item2)
{
    var __this = this;
    __this.Items = [Item0, Item1, Item2];
});
TupleFSharpFunc_2_WebResponse__Unit__FSharpFunc_2_Exception__Unit__FSharpFunc_2_String__Unit_ = (function(Item0,Item1,Item2)
{
    var __this = this;
    __this.Items = [Item0, Item1, Item2];
});
TupleFSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleFSharpOption_1_Result_1_Error____FSharpOption_1_Result_1_LintWarning___ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_IPane_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleIEnumerable_1_Tuple_2_IPane__Object__IEnumerator_1_Object_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleIPane__Int32 = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleIPane__Object_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleJQuery__FSharpList_1_JQuery_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleObject__Int32 = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleObject____FSharpList_1_Object___ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleString_String = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleString____FSharpList_1_String___ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleString____Int32 = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleTuple_2_IPane__Object__FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
TupleTuple_2_IPane__Object__FSharpOption_1_Tuple_2_IPane__Object_ = (function(Item0,Item1)
{
    var __this = this;
    __this.Items = [Item0, Item1];
});
UTF8Encoding__GetBytes$ = (function(__,text)
{
    var str = text;
    var byteArray = [];
    for (var i = 0; i < str.length; i++)
        if (str.charCodeAt(i) <= 0x7F)
            byteArray.push(str.charCodeAt(i));
        else {
            var h = encodeURIComponent(str.charAt(i)).substr(1).split('%');
            for (var j = 0; j < h.length; j++)
                byteArray.push(parseInt(h[j], 16));
        }
    return byteArray;;
});
UTF8Encoding__GetString$ = (function(__,bytes)
{
    var byteArray = bytes;
    var str = '';
    for (var i = 0; i < byteArray.length; i++)
        str +=  byteArray[i] <= 0x7F?
                byteArray[i] === 0x25 ? "%25" : // %
                String.fromCharCode(byteArray[i]) :
                "%" + byteArray[i].toString(16).toUpperCase();
    return decodeURIComponent(str);;
});
UTF8Encoding___ctor$ = (function(unitVar0)
{
    {};
});
UnfoldEnumerator_2_Boolean__Tuple_2_IPane__Object___ctor$Boolean_Tuple_2_IPane__Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_FSharpList_1_Object____Object_____ctor$FSharpList_1_Object____Object___ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_FSharpList_1_String____String_____ctor$FSharpList_1_String____String___ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_IEnumerator_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object___ctor$FSharpOption_1_Tuple_2_IPane__Object__Tuple_2_IPane__Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_IPane__IEnumerable_1_Tuple_2_IPane__Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object___ctor$IEnumerator_1_Object__IEnumerable_1_Tuple_2_IPane__Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_Int32__Disposable___ctor$Int32_Disposable_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_Int32__IPane___ctor$Int32_IPane_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_Int32__Object___ctor$Int32_Object_ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UnfoldEnumerator_2_Int32__String_____ctor$Int32_String___ = (function(seed,unfold)
{
    var __this = this;
    {};
    __this.seed = seed;
    __this.unfold = unfold;
    __this.acc = {Tag: 1.000000, Value: __this.seed};
    __this.current = null;
});
UpdateGeneratorRequest___ctor$ = (function(Blank)
{
    var __this = this;
    __this.Blank = Blank;
});
WebHeaderCollection__Add$ = (function(__,key,value)
{
    __.headers = List__CreateCons$Tuple_2_String__String_Tuple_2_String__String_((new TupleString_String(key, value)), __.headers);
});
WebHeaderCollection___ctor$ = (function(unitVar0)
{
    var __this = this;
    {};
    __this.headers = List__Empty$Tuple_2_String__String_Tuple_2_String__String_();
});
WebHeaderCollection__get_Keys$ = (function(__,unitVar1)
{
    return List__ToArray$String_1String(List__Map$Tuple_2_String__String__String_1Tuple_2_String__String__String((function(tuple)
    {
      return tuple.Items[0.000000];
    }), __.headers));
});
WebHeaderCollection__get_Values$ = (function(__,unitVar1)
{
    return List__ToArray$String_1String(List__Map$Tuple_2_String__String__String_1Tuple_2_String__String__String((function(tuple)
    {
      return tuple.Items[1.000000];
    }), __.headers));
});
WebRequest__AsyncGetResponse$ = (function(req,unitVar1)
{
    return Async_1_FromContinuations$WebResponse_WebResponse_((function(tupledArg)
    {
      var onSuccess = tupledArg.Items[0.000000];
      var onError = tupledArg.Items[1.000000];
      var _arg1 = tupledArg.Items[2.000000];
      var matchValue = _arg1;
      var onReceived = (function(data)
      {
        var bytes = UTF8Encoding__GetBytes$(Replacements__utf8Encoding$(), data);
        return onSuccess((new WebResponse___ctor$(bytes)));
      });
      var onErrorReceived = (function(unitVar0)
      {
        return onError(null);
      });
      var _1025;
      if ((WebRequest__get_Method$(req) == "GET")) 
      {
        _1025 = null;
      }
      else
      {
        _1025 = UTF8Encoding__GetString$(Replacements__utf8Encoding$(), Stream__get_Contents$(req.requestStream));
      };
      var body = _1025;
      return Web__sendRequest$Unit_Unit_(WebRequest__get_Method$(req), req.url, WebHeaderCollection__get_Keys$(WebRequest__get_Headers$(req)), WebHeaderCollection__get_Values$(WebRequest__get_Headers$(req)), body, onReceived, onErrorReceived);
    }));
});
WebRequest__Create$ = (function(uri)
{
    return (new WebRequest___ctor$(uri));
});
WebRequest__GetRequestStream$ = (function(__,unitVar1)
{
    return __.requestStream;
});
WebRequest___ctor$ = (function(url)
{
    var __this = this;
    {};
    __this.url = url;
    __this.requestStream = (new Stream___ctor$([], (function(value)
    {
      var ignored0 = value;
    })));
    __this.Url_ = __this.url;
    __this.Headers_ = (new WebHeaderCollection___ctor$());
    __this.Method_ = "GET";
});
WebRequest__get_Headers$ = (function(__,unitVar1)
{
    return __.Headers_;
});
WebRequest__get_Method$ = (function(__,unitVar1)
{
    return __.Method_;
});
WebRequest__set_Method$ = (function(__,v)
{
    __.Method_ = v;
});
WebResponse__GetResponseStream$ = (function(__,unitVar1)
{
    return (new Stream___ctor$(__.contents, (function(value)
    {
      var ignored0 = value;
    })));
});
WebResponse___ctor$ = (function(contents)
{
    var __this = this;
    {};
    __this.contents = contents;
});
Web__sendRequest$Unit_Unit_ = (function(meth,url,headerKeys,headerValues,body,onSuccess,onError)
{
    
    var _method = meth, 
        _url = url, 
        _headerKeys = headerKeys, 
        _headerValues = headerValues,
        _body = body, 
        _onSuccess = onSuccess, 
        _onError = onError;

    if (window.XDomainRequest) {
        var req = new XDomainRequest();
        req.onload = function() { _onSuccess(req.responseText); };
        req.onerror = _onError;
        req.ontimeout = _onError;
        req.timeout = 10000;
        req.open(_method, _url);
        if(_body) {
            req.send(_body);
        } else {
            req.send();
        }
    }
    else {
        var req;

        if (window.XMLHttpRequest)
          req = new XMLHttpRequest();
        else
          req = new ActiveXObject("Microsoft.XMLHTTP");

        req.onreadystatechange = function () {
            if(req.readyState == 4) {
                if(req.status == 200 || req.status == 204) {
                    _onSuccess(req.responseText);
                }
                else {
                    _onError();
                }
            }
        };
        req.open(_method, _url, true);
        for(var i = 0; i < _headerKeys.length; i++) {
            var key = _headerKeys[i];
            var value = _headerValues[i];
            req.setRequestHeader(key, value);
        }
        if(_body) {
            req.send(_body);
        } else {
            req.send();
        }
    };
});
list_1_JQuery__ConsJQuery_ = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_JQuery__NilJQuery_ = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
list_1_Object____ConsObject___ = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_Object____NilObject___ = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
list_1_String__ConsString = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_String__NilString = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
list_1_String____ConsString___ = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_String____NilString___ = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
list_1_Tuple_2_IPane__Object__ConsTuple_2_IPane__Object_ = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_Tuple_2_IPane__Object__NilTuple_2_IPane__Object_ = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
list_1_Tuple_2_String__String__ConsTuple_2_String__String_ = (function(Item1,Item2)
{
    var __this = this;
    __this.Tag = 1.000000;
    __this._CaseName = "Cons";
    __this.Item1 = Item1;
    __this.Item2 = Item2;
});
list_1_Tuple_2_String__String__NilTuple_2_String__String_ = (function()
{
    var __this = this;
    __this.Tag = 0.000000;
    __this._CaseName = "Nil";
});
Logger__active = Logger__get_active$();
Logger__logPath = Logger__get_logPath$();
Logger__editor = Logger__get_editor$();
Logger__subscriptions = Logger__get_subscriptions$();
LanguageService__service = LanguageService__get_service$();
Logger__fullLog = Logger__get_fullLog$();
LanguageService__port = LanguageService__get_port$();
return [(function(ign)
{
    return (new TextOnIDE___ctor$());
}), (function(_this)
{
    return TextOnIDE__deactivate$(_this);
}), (function(_this)
{
    return (function(p0)
    {
      return TextOnIDE__activate$(_this, p0);
    });
}), (function(_this)
{
    return TextOnIDE__provideErrors$(_this);
})]
 }
var _funcs = wrappedFunScript();
var _self = _funcs[0]();

module.exports = TextOn = {
deactivate: function() {
  return _funcs[1](_self); },
activate: function(p1) {
  return _funcs[2](_self)(p1); },
provideErrors: function() {
  return _funcs[3](_self); },
config:  {
                UseLinter: {type: 'boolean', 'default': true},
                DeveloperMode: {type: 'boolean', 'default': false},
                TextOnPath: {type: 'string', 'default': ''},
                MonoPath: {type: 'string', 'default': '/usr/bin'},
GeneratorConfig: {type: 'object', properties: {
                    NumSpacesBetweenSentences : {type: 'number', 'default': 2 },
                    NumBlankLinesBetweenParagraphs : {type: 'number', 'default': 1 },
                    WindowsLineEndings : {type: 'boolean', 'default': false }
                   }}
}
};