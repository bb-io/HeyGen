# HeyGen 1.4.0 technical documentation

## Connection and access

Generate a key in the [HeyGen API dashboard](https://platform.heygen.com/api-keys). In Blackbird, add a **HeyGen** connection and paste the key. All authenticated requests go to `https://api.heygen.com/v3/` with `x-api-key`. `GET /v3/api_keys/self` validates the connection without requiring `account:read`. This verifies the key itself. Resource ownership, each action's scope, the workspace's entitlements and the API balance determine what the key can do.

| Capability | Minimum API key scopes |
|---|---|
| Connect | A valid API key |
| Search, get, download and poll videos, read scripts | `videos:read` |
| Create avatar or Studio video | `videos:write` |
| Upload Studio scene media or narration files | `assets:write` for the file upload |
| Pick avatar group, look or its engine | `avatars:read` |
| Pick voice | `voices:read` |
| Text to speech or Speech to speech | `voices:write` |
| Pick or inspect templates, export template XLIFF | `templates:read` |
| Render a template with or without XLIFF | `templates:read`, `videos:write` |
| Submit native translation | `translations:write` |
| Start proofread, upload edited SRT, or generate final dub | `translations:write` |
| Get proofread session, select session, download SRT, or poll readiness | `translations:read` |
| Upload a Blackbird SRT file to HeyGen | `assets:write` in addition to `translations:write` |
| Get, download or poll translation, discover languages | `translations:read` |
| Submit lipsync or read results | `lipsync:write` or `lipsync:read`, respectively |
| Submit AI clipping or read results | `ai_clipping:write` or `ai_clipping:read`, respectively |
| Submit Video Agent or read sessions and styles | `video_agent:write` or `video_agent:read`, respectively |
| Pick, search or get glossaries | `brand:read` |
| Create, update or delete a glossary | `brand:write` |
| Upload Blackbird source media or Agent reference files | `assets:write` in addition to the job scope |
| Create or read a video, translation, lipsync or asset batch | Write or read scope for the corresponding resource |
| Resolve an existing video ID as a job's media source | `videos:read` in addition to the job scope |

HeyGen's write scope also satisfies the read scope for the same resource. Public catalogue avatars and voices still use authenticated catalogue endpoints. Public source media URLs must be directly downloadable by HeyGen without credentials or redirects. URLs in responses can be temporary presigned URLs. Blackbird refreshes them by ID before downloading. API keys with workspace access can use the resources HeyGen exposes to them. Private Agent sessions are owner-visible. Admin and owner rules, including who can issue a key or use a restricted workspace asset, are administered in HeyGen. The app has no workspace admin operations and exposes no permission-escalation mechanism. Eligible commercial API usage requires a funded API balance or contract. IPA pronunciations, private assets, avatars, premium translation dialects and model eligibility depend on the workspace. See [API key scopes](https://developers.heygen.com/docs/api-key-permissions), [API keys](https://developers.heygen.com/docs/api-key), and [glossary pronunciation](https://developers.heygen.com/docs/brand-glossary/pronunciations).

## Action reference

Provider paths below are relative to `/v3`. Video and batch creation actions return after submission. The two audio conversion actions wait for the audio file. Map the returned ID into a status action or use the relevant polling trigger. Read failure fields before running downstream delivery. Each download returns a Blackbird file reference.

| Action | API and output |
|---|---|
| Search videos | `GET /videos`. Returns video details including ID, title, status, timestamps and artifact links when present. Title is matched by HeyGen and status is applied before the result cap. |
| Get video | `GET /videos/{id}`. Returns video ID, title, status, duration, video and captioned video URLs, subtitle/thumbnail/page URLs, timestamps and failure information. |
| Download video | Reads `video_url` from Get video and downloads the render. The returned render may contain burned-in captions if requested when it was created. |
| Get captions | Reads `subtitle_url` from Get video and downloads the SRT sidecar. Availability depends on the artifact returned by HeyGen. |
| Create video | `POST /videos` with type `avatar`. Submits a single-scene render from an avatar look and script. Returns the video ID and submission status. Limit 5,000 script characters. |
| Create Studio video | `POST /videos` with `type: studio`. Composes 1–50 ordered scenes. Returns a video ID and initial status for existing status and download actions. |
| Text to speech | `POST /models/audio/tts`. Returns a WAV or MP3 Blackbird file, duration, request ID and spoken language when returned by HeyGen. Uses the standard HeyGen Voice 1 model. |
| Speech to speech | `POST /models/audio/sts`. Revoices a recording with a workspace instant voice, retaining the original words and timing. Returns a WAV Blackbird file, duration and request ID when present. |
| Create avatar video from audio | `POST /videos` with `audio_asset_id` or `audio_url`. Returns the video ID and submission status. Use Get video, the existing completion event, or Download video downstream. |
| Get template variables | `GET /templates/{id}`. Returns template ID, name and JSON describing variable types and defaults. |
| Create video from template | Reads the template and submits `POST /templates/{id}`. Returns video metadata and render ID. Supplies current text defaults and applies explicit replacements. Other unchanged variables keep their existing template values. |
| Translate video | `POST /video-translations`. Uses HeyGen's native dubbing and returns one translation ID per requested output language plus any warnings. |
| Get video translation | `GET /video-translations/{id}`. Returns status, target language, video/audio/caption URLs, timestamps, correlation and failure information. |
| Download translated video | Downloads the current translated video URL. An audio-only job may expose only audio for downstream use. |
| Start translation proofread | `POST /video-translations/proofreads`. Starts subtitle preparation and returns a proofread session ID per language plus the initial status. |
| Get proofread session | `GET /video-translations/proofreads/{id}`. Returns session ID, title, source and target languages, preparation status and failure message. |
| Download proofread SRT | `GET /video-translations/proofreads/{id}/srt`. Downloads the session's current translated SRT as a Blackbird file. |
| Upload edited proofread SRT | `PUT /video-translations/proofreads/{id}/srt`. Replaces the session's SRT and returns updated session details. Uploads Blackbird files to HeyGen first. |
| Generate video from proofread | `POST /video-translations/proofreads/{id}/generate`. Starts the final dubbed render and returns a translation ID and initial status. |
| Get translation captions | Downloads SRT or WebVTT from the current translation details. Defaults to SRT. |
| Export video script as XLIFF | `GET /videos/{id}/scenes`. Reads editable synthesized speech and returns an XLIFF 1.2 file. |
| Read translated video script | Reads the XLIFF, rechecks the source with `GET /videos/{id}/scenes`, and returns translated script, source video ID and target language tag. Map the script into Create video. |
| Export template text as XLIFF | `GET /templates/{id}`. Returns XLIFF 1.2 containing the non-empty default text variables. |
| Create template video from XLIFF | Reads translated text, checks the selected template's current defaults and submits `POST /templates/{id}`. Returns the newly submitted video and ID. |
| Search brand glossaries | `GET /brand-glossaries`. Returns up to 100 accessible glossary IDs and names. |
| Get brand glossary | `GET /brand-glossaries/{id}`. Returns name and the three rule lists as JSON. |
| Create brand glossary | `POST /brand-glossaries`. Returns the new glossary ID and rules. Workspace name must be unique. |
| Update brand glossary | `PATCH /brand-glossaries/{id}`. Each supplied list replaces the whole previous list. Omit it to keep it or pass `[]` to clear it. |
| Delete brand glossary | `DELETE /brand-glossaries/{id}`. Permanently deletes a glossary and returns its ID. |
| List Video Agent drafts | `GET /video-agents/{session_id}/videos`. Returns the session's drafts newest first with IDs, status, and timestamps. |
| List video scenes | `GET /videos/{video_id}/scenes`. Returns scene IDs, sequence numbers, text narration and the draft edit version. |
| Revise selected scenes | `POST /video-agents/{session_id}` with `edit_plan`. Accepts one scene and instruction or a list of edits for one reviewed draft. Returns an agent run ID and the new working draft's video ID when available. |
| Create Video Agent session | `POST /video-agents` in private, one-shot generation mode. Returns session ID, state and video ID when assigned. |
| Get Video Agent session | `GET /video-agents/{id}`. Returns state, progress, failure details when provided, and video ID. A session can pause for input. This integration covers one-shot sessions. |
| Create lipsync | `POST /lipsyncs`. Submits a source video and replacement audio. Returns a lipsync ID. |
| Get lipsync | `GET /lipsyncs/{id}`. Returns state, video/caption URLs and failure information. |
| Download lipsync video | Downloads the currently available lipsync render as a Blackbird file. |
| Create AI clipping job | `POST /ai-clipping`. Submits a source, optional length bands, framing, captions and editorial guidance. Returns a clipping job ID. |
| Get AI clipping job | `GET /ai-clipping/{id}`. Returns status, progress, clips, durations, virality scores, URLs and failure information. |
| Download clip | Reads the selected job and downloads the selected completed clip. |
| Create batch | `POST /videos/batches`, `/video-translations/batches` or `/lipsyncs/batches`. Accepts 1–100 native request objects. Returns batch type and ID. A translation request with multiple languages expands into individual jobs. |
| Create asset batch | `POST /assets/direct-uploads/batches`, then streams each Blackbird file to its signed upload slot and calls `POST /assets/complete/batches`. Returns `assets` and a batch ID. |
| Complete asset batch | `POST /assets/complete/batches`. Re-drives ingestion for the same batch after all uploads completed. |
| Get batch | `GET /{batch family}/batches/{id}`. Returns aggregate state, total item count and per-item request index, job ID, status and failure message. For asset batches the job ID is the asset ID. |

The asset-batch action accepts MP4, WebM, MP3, WAV, PNG, JPEG and PDF, with an app cap of 200 MiB per file. Other supported direct-upload types and higher HeyGen limits are outside this action's profile. The app sends slot headers to the storage endpoint, without the HeyGen API key. If uploads are interrupted while the slots are valid, reuse the same submission key and unchanged files in the original order. If ingestion alone needs resuming, use **Complete asset batch** with the ID. Ingestion is per item. The smaller multipart upload used by media actions accepts up to 50 MB, or 32 MB for images. For bigger source media, use an existing asset ID, asset batch or an HTTPS URL appropriate for the job. See [HeyGen assets](https://developers.heygen.com/assets) and [asset batches](https://developers.heygen.com/batch-assets).

### Downloaded media and captions

The connector copies the HeyGen download response to a temporary file and passes a rewound stream with a known length to Blackbird File Management. This handles forward-only HTTP responses with or without a Content-Length header. The temporary file is removed when the stream closes. The connector uses disk for the transfer so large videos do not require buffering the entire file in memory.

## Input-design audit

The following covers the visible inputs. Read scopes refer to the connection used for discovery. Paste or map the displayed ID's underlying value where Blackbird exposes manual entry. Actions accept valid runtime values regardless of picker membership. A picker can show a permissions error without preventing a Bird with a mapped ID and adequate execution scope from using the action. Requiredness follows the SDK input model. Blackbird UI behavior still needs an installation test.

| Field or repeated fields | Control and source | Parent or scope | Default and fallback |
|---|---|---|---|
| API key | Secret text | HeyGen API dashboard. Use the intended workspace. | Required. |
| Title contains, video Status, Maximum results | Text, fixed status picker, integer | Search videos uses `videos:read`. Status choices come from OpenAPI. | No title/status filter. 100 results by default, range 1–1,000. |
| Video ID, including event filter and existing source | Searchable video dropdown, `GET /videos` | `videos:read`. Search by title substring at HeyGen. | Required for read/download/export. Optional for events and among source choices. Paste or map the full ID for direct lookup. The picker shows a usable title with date and duration when available. Untitled and duplicate-title entries include a short ID suffix. |
| Avatar group ID, Avatar ID | Searchable group and look dropdowns, `GET /avatars` and `GET /avatars/looks` | `avatars:read`. Group selection filters looks. An empty group browses accessible looks. | No group default. Choose a look for Create video. Video Agent may choose an avatar when no look is supplied. Paste a look ID. |
| Avatar engine | Dependent dropdown, `GET /avatars/looks/{id}` | Selected avatar look and `avatars:read`. Displays engines published as supported by that look. | Empty without a look. If omitted, HeyGen defaults to Avatar IV. Supply a documented compatible engine value manually if needed. |
| Voice ID | Searchable provider voice dropdown | `voices:read`, workspace active clones and public HeyGen voices. | Avatar's default voice for Create video when omitted. Agent selects narration when omitted. Paste an eligible voice ID. |
| Aspect ratio, Resolution, Burn captions | Fixed ratio and 720p/1080p choices, and Boolean | Create video uses documented avatar render settings. | Provider defaults of 16:9, 720p and sidecar-only captions. A mapped 4K value requires an eligible Avatar III look. Native batches also accept eligible combinations. |
| Script text | Text | Create video, avatar look required. | Required. Map translated script or enter up to 5,000 characters. |
| Template ID, Text variable name | Searchable template and variable dropdowns | `templates:read`. Text-variable picker reads selected template only. | Template ID required. Text variable optional and paired with Replacement text. Missing parent gives no options. Map template ID or exact variable name. |
| Replacement text, Additional variable replacements | Text and JSON object | Selected template governs accepted keys and types. | Current text defaults are filled for omitted variables. Explicit text field wins over JSON for the same key. |
| Template Burn captions | Boolean | Selected template | Omit to use HeyGen behavior. Template submit explicitly leaves public sharing off. |
| Source video file, URL, asset ID or Video ID | Blackbird file, URL text, asset ID text or video picker | Exactly one for Translate video, Create lipsync and Create AI clipping job. `assets:write` for file upload, `videos:read` for Video ID. | No default. Map files, URLs or an asset ID from Create/Get batch. |
| Replacement audio file, URL or asset ID | Blackbird file and text | Exactly one for Create lipsync. `assets:write` to upload a file. | No default. Use MP3/WAV for Blackbird files. Map a known asset ID or direct URL. |
| Target languages | Searchable multi-choice, `GET /video-translations/languages` | `translations:read`. Uses plan-specific names when provided. | One or more targets required. Store exact provider display names such as `Spanish (Spain)`. Map a supported name. |
| Mode, Translate audio only | Fixed quality picker, Boolean | Translation and lipsync modes use documented values. Audio only belongs to native translation. | HeyGen defaults to Speed with ordinary lip-synced translation. |
| Translation ID, Lipsync ID, Clipping job ID, Agent session ID | Searchable connection-scoped dropdowns from their list endpoints | `translations:read`, `lipsync:read`, `ai_clipping:read`, `video_agent:read` respectively. | Map the ID from the corresponding create action. Translation ID is optional on the translation trigger. |
| Caption format | Fixed SRT/WebVTT picker | Translation ID | SRT. |
| Clip ID | Dependent searchable dropdown from clipping details | Clipping job ID and `ai_clipping:read`. Completed clips only. | Empty without a parent or completed clips. Map the exact clip ID from Get AI clipping job. |
| Source language code, Clip aspect ratio, Target clip lengths, Burn captions into clips | Maintained shortlist of ISO 639-1 codes, documented fixed framing/length options, Boolean | Clipping. The shortlist is not account discovery. | HeyGen auto-detects source language. Portrait and captions on by default. Omit duration to use provider defaults. Map other valid source codes directly. |
| Editorial guidance, Prompt | Text | Clipping and Video Agent respectively. | Clipping guidance optional, up to 500 characters. Agent prompt required, 1–10,000 characters. |
| Agent style ID, Orientation | Searchable live style picker and fixed orientation | `video_agent:read` for styles. Orientation settings are independent. | Agent selects style/orientation if omitted. Paste a known style ID. |
| Reference files | Blackbird file list | `assets:write`. Agent supports MP4, WebM, MP3, WAV, PNG, JPEG and PDF here. | Optional, up to 20 files. Private session is enforced. |
| Brand glossary ID | Searchable workspace dropdown, `GET /brand-glossaries` | `brand:read` to discover. Pass glossary to applicable render, translation or Agent action. | Required for get/update/delete. Optional for job creation. Map a known ID. |
| Glossary name, Pronunciations, Do-not-translate rules, Forced translations | Text and typed-as-text JSON arrays | Workspace glossary. Existing contents available via Get brand glossary. | Name required on create. No rules by default. Update omits absent fields and replaces supplied lists. |
| Batch type, Batch ID, Job inputs | Fixed batch family, ID text and JSON array | Batch family selects the endpoint. HeyGen documents read-by-ID endpoints for these batch families. | Required. Map both returned fields from Create batch or Create asset batch. |
| Files, Batch title | Blackbird file list and text | Asset batch, `assets:write`. | Files required, 1–100 in input order. Titles optional on both asset and job batches. |
| Script entry ID | Searchable dependent dropdown, `GET /videos/{id}/scenes` | Video ID and `videos:read`. Lists available synthesized speech with scene context. | Empty without video. Omit to export all non-blank speech entries, or map one ID. |
| Source language tag, Target language tag | Text, BCP 47-style tags | Both required for XLIFF export. Different format from native HeyGen language names. | No language inferred. Example `en`, `pt-BR`. |
| Translated XLIFF file | Blackbird file | Imports the exported XLIFF 1.2 and rechecks its source. Template route also requires Template ID. | Required. Map approved TMS output. File cap 2 MB. |
| Title, Correlation ID, Submission key | Text | Title and correlation are accepted only by documented payloads. The submission key is the provider's `Idempotency-Key`. | Title and correlation optional. Submission key required for keyed creates. Map a stable job-specific value. |

Picker searches return bounded results. Server-side search is used for video titles and public voices. Most other provider lists are filtered locally across up to 15 cursor pages. If the cap is reached before enough matches or exhaustion, the picker surfaces an incomplete-search error. Narrow a query where the provider supports it, select a parent, or paste/map the known ID. Job and asset batch IDs remain text because there is no documented general batch lookup by name. Audio/video asset IDs remain text because the public asset search catalogue does not enumerate these reusable assets. The app does not require an account or folder selector. HeyGen derives the workspace from the key and creates renders in the default workspace destination.

## Audio production inputs and limits

| Action | Inputs and defaults |
|---|---|
| Text to speech | Text (required, up to 10,000 characters), searchable voice ID (active public or workspace voice), optional base language such as `en` or `hu` (blank lets HeyGen detect it), and WAV or MP3 (defaults to WAV). Locale variants such as `pt-BR` are unsupported on this model. |
| Speech to speech | Active instant voice ID from the connected workspace, one audio source, a stable submission key, and optional background noise removal (off by default). The output is always WAV. The picker excludes public voices and professional clones. |
| Create avatar video from audio | Avatar look, one audio source, and a stable submission key. Supports the existing avatar-group and engine pickers, aspect ratio, resolution, optional title, burned-in captions and correlation ID. |

For either source-audio action, select exactly one Blackbird WAV/MP3 file, direct HTTPS URL, or existing HeyGen audio asset ID. Blackbird file upload requires `assets:write` and uses the 50 MB multipart limit. The speech-to-speech API caps recordings at five minutes, with up to 32 MB by URL and 100 MB by existing asset ID. Avatar audio is limited to ten minutes by HeyGen. A URL must point directly to media that HeyGen can download. Map a known voice or asset ID if the workflow already has it. See [speech-to-speech inputs](https://developers.heygen.com/docs/voices/speech-to-speech) and [HeyGen usage limits](https://developers.heygen.com/docs/usage-limits).

Both speech actions run synchronously. They use a 170-second synthesis request timeout, followed by a separate file transfer into Blackbird. Speech-to-speech can take up to 150 seconds. For `voice_not_ready`, `request_in_progress`, or an unknown outcome, wait for the provider before reusing the same key and identical inputs within 24 hours. A different body with the same speech conversion key can return a conflict. Text-to-speech exposes no documented idempotency header. Check HeyGen usage before repeating a text-to-speech request with an unknown outcome. The app does not automatically retry paid requests.

**Example Bird.** Map an approved TMS script into **Text to speech**, route its audio file to a Checkpoint for pronunciation and pacing, then map the approved file to **Create avatar video from audio**. Use a stable key derived from the approved asset and revision. For footage you already have, start with a scratch recording and map the WAV from **Speech to speech** into the existing **Create lipsync** action.

### Video picker labels

Video pickers display the HeyGen title first, followed by the creation date and duration if supplied. If the title is missing or matches the raw ID, the picker checks details for up to five such results. When HeyGen still provides no usable title, the picker displays **Untitled video** and a short ID suffix. Duplicate titles also include a short ID suffix. Blackbird stores the full ID for every choice, so saved selections and step mappings retain their existing value. The picker continues to use HeyGen’s title search for discovery. Specify **Title** when creating a video to make future selection easier.

## Review and revise Video Agent drafts

A Video Agent session may contain multiple cuts. Select a completed draft, then use **List video scenes** to read the ordered scenes. Map its **Draft video ID** and **Edit version** from the draft the reviewer actually saw. The **Scene ID** picker in **Revise selected scenes** shows the scene number and narration preview for the chosen draft. Use one scene and instruction or map up to 50 unique scene edits. Each instruction can use up to 10,000 characters.

Submit the confirmed edits with a durable submission key. A retry of the same approved plan uses the same key. A new approval or rebase uses a new key. A stale edit version sends the draft back for review. The version is a snapshot token and does not serialize concurrent edits, so sequence submissions for the same session. HeyGen issues new scene IDs for the resulting draft, so list the scenes again for any further revision.

Poll **Get video** using the returned **Video ID**. If no video ID is returned, use the Agent run ID for troubleshooting, check the session, and identify the right draft in its history before continuing. Preserve the reviewed draft and the revised draft as separate IDs. See [HeyGen scene editing](https://developers.heygen.com/docs/edit-scenes) and [draft history](https://developers.heygen.com/reference/list-session-videos).

## Studio video composition

Map an ordered array of scene objects to **Create Studio video** using **Scenes**. Alternatively, map a JSON array to **Scenes JSON** when inputs are scripts, URLs or asset IDs. Choose just one input. The action validates scene combinations before uploading files. Choose **Automatic per Flight** or supply a custom submission key. Existing mappings use Custom when mode is omitted. Automatic derives a provider key from the Blackbird Flight ID and **Render slot**. For one render in the Flight, the slot can be blank. For multiple render steps or loop iterations, supply a distinct stable slot per render, such as a step label plus a source item ID. Reuse the same slot when retrying a render within that Flight. A new Flight generates a different key. Use Custom to deliberately deduplicate across Flights. The action uses separate derived keys for media uploads at each scene position. Use a fresh custom key for a new revision or scene order.

| Scene type | Required inputs | Optional behavior |
|---|---|---|
| Speaking avatar (`avatar_video`) | Avatar look ID and either script or one narration source | Voice ID for script. Omit to use the avatar's saved voice. |
| Still image (`image`) | Exactly one image source, plus duration above 0 and up to 300 seconds for a silent still, or a script or narration source for a narrated still | Specify a voice ID for script. Omit duration when narrated. |
| Video clip (`video`) | Exactly one video source | Leave narration empty to play the original clip, or supply a script with voice ID or one narration source. Set mute to silence original clip audio. |

For a visual or narration source, choose a Blackbird file, a direct URL HeyGen can fetch, or an existing HeyGen media asset ID. The file path supports JPG/PNG images (32 MB max), MP4/WebM footage, and MP3/WAV narration (50 MB max). Metadata and duration limits can also apply on the provider side. Use the avatar look ID from an existing HeyGen avatar action or connection context. The canvas ratio, resolution, brand glossary and captions are global. The Studio defaults are 16:9 and 1080p. The glossary applies to HeyGen-generated narration. HeyGen returns a sidecar when captions are available. Enable **Burn captions into the video** for a visible overlay. See [HeyGen Studio composition](https://developers.heygen.com/studio-videos).

## Native translation proofreading

| Action | Inputs and defaults |
|---|---|
| Start translation proofread | Title, one or more target languages, exactly one source video, and a submission key. Source choices are a Blackbird file, direct video URL, existing HeyGen asset ID, or completed HeyGen video ID. Uses the existing target-language picker. Mode defaults to Speed. Precision and a brand glossary are optional. |
| Get proofread session and Download proofread SRT | A proofread session ID. Pick from the connected workspace by title, ID or target language, or map the ID returned by the start action. The picker displays the preparation status. |
| Upload edited proofread SRT | The same session ID and exactly one approved subtitle source. Use a Blackbird .srt file up to 50 MB, a direct HTTPS URL HeyGen can fetch, or an existing HeyGen SRT asset ID. |
| Generate video from proofread | The same session ID and a separate submission key for the approved revision. Renders from the SRT currently saved to the session. |

Use a paid HeyGen API account. Both Speed and Precision can prepare translated subtitles for review. A session's `completed` status means the subtitles are ready. The separate final video translation status describes render progress. The action downloads the current translated SRT, including any edits already saved in HeyGen. Keep each session paired with its target language and keep edits in valid SRT format. See [HeyGen proofreading](https://developers.heygen.com/docs/video-translate#proofread-before-finalizing) and [HeyGen API pricing](https://help.heygen.com/en/articles/10060327-heygen-api-pricing-credits-explained).

**Example Bird.** Start translation proofread → wait with On proofread ready scoped to each session → Download proofread SRT → send it to a linguist through a Checkpoint or TMS → Upload edited proofread SRT → Generate video from proofread → Get video translation → Download translated video. For several target languages, loop over the returned sessions and use Get proofread session to confirm the target language before routing each SRT. Use **Get translation captions** for the final render's SRT or WebVTT.

Keep one active review revision per session. Map the session ID from the successful upload into the render step and wait for upload to finish before submitting the render. HeyGen's SRT upload is a replacement with no documented revision check. Avoid parallel edits on the same session and avoid resubmitting an older file after a newer approval. Start and final rendering take provider submission keys. Give them distinct keys and reuse a key only to retry the identical logical request within 24 hours.

## Native versus TMS workflow

**Native dubbing.** Choose a completed HeyGen video, a Blackbird source file, a direct URL or an asset ID. **Translate video** submits voice-cloned dubbing in Speed or Precision. One ID is returned for each target language. Use **Get video translation** or **On translation completed** to route the result. **Download translated video** and **Get translation captions** retrieve artifacts. This operation has no embedded human checkpoint. For a human review before final rendering, use native translation proofreading below.

**TMS with reviewed template text.** In Studio, expose translatable script and screen-text phrases as template text variables and give them meaningful source defaults. Run **Export template text as XLIFF** with explicit source and target tags. Route the file to the TMS, wait at a Blackbird Checkpoint, and map the approved XLIFF, original template ID and submission key into **Create template video from XLIFF**. Follow with **On video completed** or **Get video**, then **Download video**. Fixed text, footage and other design elements stay as defined in the current template.

**TMS with reviewed spoken script.** Run **Export video script as XLIFF** on a video with editable scene speech. Review in the TMS, then run **Read translated video script**. Map the returned script into **Create video**, and explicitly select the avatar and any target-language voice. This is a new single-scene render. The file does not carry scene editing instructions, recorded audio or spoken timing. For a longer or multi-scene re-render, use prepared templates or split scripts into renders that stay within the 5,000-character limit. A script entry shared by multiple scenes is exported once.

Keep the original reference, unit IDs, source text, target language and all markup or placeholders. Use plain-text targets. The importer checks for source changes and missing translations on the template route. For video imports, a file can contain an intentional subset of source entries. Older videos, rendered-only media and unfinished editor documents may lack usable scene text. XLIFF changes text for a new render inside the Bird. HeyGen does not accept the XLIFF file as a native edit.

## Events and recovery

| Trigger | Read endpoints | Output |
|---|---|---|
| On video completed | `GET /videos` or `GET /videos/{id}` | Video details for newly completed renders |
| On video failed | Same video endpoints | Video details for newly failed renders, including provider failure fields |
| On translation completed | `GET /video-translations` or `GET /video-translations/{id}` | Translation details for newly completed jobs |
| On proofread ready | `GET /video-translations/proofreads` or `GET /video-translations/proofreads/{id}` | A list of completed proofread sessions with editable SRTs |

HeyGen supports signed push webhooks and recommends verifying raw-body HMAC. Its per-endpoint secret is returned only at creation and rotation. The SDK subscription interface verified for this package exposes no documented durable secret handoff from activation to delivery. This version uses Blackbird polling and needs no HeyGen webhook scope, endpoint or signing secret. See [HeyGen webhooks](https://developers.heygen.com/docs/webhooks) and [Blackbird events](https://docs.blackbird.io/sdk/events/).

Set the event interval in Blackbird. For workspace-wide triggers, the first successful run establishes a baseline, including jobs still in progress. It emits no historical jobs. Later successful runs compare terminal statuses and look back five minutes for late-visible jobs. A failed read returns no updated state. A changed filter establishes a new baseline. Polling assumes a created timestamp for newly discovered jobs that finished before they were first observed. A previously observed pending job can transition without a creation timestamp.

For **On proofread ready**, leave the ID blank for workspace-wide polling. A first workspace poll records a baseline and skips already-ready sessions. When used as a Checkpoint, bind the specific session ID returned by **Start translation proofread**. A scoped poll can emit an already-ready session on its first check and suppresses repeats for that polling state. A scoped failed session reports the provider failure. The event output remains a collection with one item when scoped.

Account-wide polling scans up to 50 pages of 100 records. If an account exceeds that limit, use an explicit job ID or a workflow that checks downstream status by ID. A poll returns a collection in one Flight. Use a Loop if each job needs independent handling. Persist the video or translation ID in external delivery systems if duplicate delivery across flow redeployments or resumed Flights would matter.

## Billing, limits and retry behavior

The README carries the self-serve per-operation rates, API credit figures, tier limits and Enterprise burst terms. Use a stable submission key for each keyed write. HeyGen replays the returned result for 24 hours for the same route and key, even if some inputs differ. Use a new key when intentionally changing the job. Correlation IDs are separate and do not establish idempotency. The app retries rate-limited reads and never retries writes automatically. For Video Agent create, which lacks a documented idempotency header, use an upstream one-time job gate if the Bird can replay a paid submission.

For a native batch, the job inputs are documented HeyGen JSON objects. For example, a video batch can use `[{"type":"avatar","avatar_id":"LOOK_ID","script":"Welcome to the rollout."}]`. A translation batch item uses `video` and `output_languages`, and a lipsync item uses `video` and `audio`. Reference public URLs or existing assets in batch JSON. Use asset batches to prepare Blackbird files for larger request lists. See the [HeyGen API specification](https://developers.heygen.com/openapi/external-api.json), [video batches](https://developers.heygen.com/batch-videos), [translation batches](https://developers.heygen.com/batch-video-translations), and [lipsync batches](https://developers.heygen.com/batch-lipsyncs).

## Install or update

Upload the deployable ZIP under **Apps → My custom apps → Create app**. Install it in a test Nest, add the API key connection, and verify a read before running paid jobs. For another version of this integration, keep `Product=HeyGen`, `PackageId=HeyGen` and `AssemblyName=Apps.HeyGen`, increase `Version` and use **New version → Update App** on the existing custom app. If the organization already has an unrelated package using those identifiers, verify its identity and bindings before updating it. Blackbird enforces unique app identifiers and increasing versions. See [Blackbird deployment](https://docs.blackbird.io/sdk/deploying/).

## Validation

The .NET 8 Release build passed. Local checks used simulated API responses to verify draft listing, scene snapshot mapping, edit payloads, stale errors, and scoped Studio key generation. The existing video download regression from 1.3.2 is retained. The 48 live integration tests were skipped because no HeyGen key was configured. The connected Blackbird session had no active organization for a live Nest test. The reviewer-to-localization demo is a configuration guide, and the complete live render, approval, proofreading, and download flow remains unverified.
