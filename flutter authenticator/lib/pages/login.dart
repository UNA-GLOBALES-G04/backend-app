import 'dart:convert';
import 'dart:io';

import 'package:another_flushbar/flushbar.dart';
import 'package:day_night_switcher/day_night_switcher.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_animated_dialog/flutter_animated_dialog.dart';
import 'package:oneauth/util/http/http_overrides.dart';
import 'package:oneauth/util/lang/language.dart';
import 'package:oneauth/util/lang_controller.dart';
import 'package:oneauth/util/theme_controller.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:wave/config.dart';
import 'package:wave/wave.dart';
import 'package:http/http.dart' as http;

/// Example app widget
class LoginPage extends StatelessWidget {
  /// Main app widget.
  const LoginPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return const LoginScreen();
  }
}

/// Example login screen
class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  bool _obscureText = true;
  bool _rememberMe = false;
  bool _validateCertificate = true;
  final urlController = TextEditingController();
  final usernameController = TextEditingController();
  final passwordController = TextEditingController();

  bool _isSigningIn = false;
  bool _isRegistering = false;

  @override
  void initState() {
    super.initState();
    _loadValidateCertificate().then((_) {
      _loadAuthUrl().then((value) {
        // TODO: auto login
      });
    });
  }

  bool get registering => _isRegistering;
  bool get signingIn => _isSigningIn;

  void setSigningIn(bool value) {
    setState(() {
      _isSigningIn = value;
    });
  }

  void setRegistering(bool value) {
    setState(() {
      _isRegistering = value;
    });
  }

  void _toggle() {
    setState(() {
      _obscureText = !_obscureText;
    });
  }

  void toggleRememberMe() {
    setState(() {
      _rememberMe = !_rememberMe;
    });
  }

  void toggleTheme() {
    ThemeController.of(context).toggleTheme();
  }

  bool setUrl(String url) {
    var uri = Uri.parse(url);
    if (uri.isAbsolute) {
      // save to shared preferences
      SharedPreferences.getInstance().then((prefs) {
        prefs.setString('url', url);
      });
      setState(() {
        urlController.text = url;
      });
      return true;
    }
    return false;
  }

  Future<String> get url async {
    var text = urlController.text;
    if (text.isEmpty) {
      var prefs = await SharedPreferences.getInstance();
      text = prefs.getString('url') ?? 'https://conecta2.rnvn.dev';
    }
    return text;
  }

  bool get validateCertificate {
    return _validateCertificate;
  }

  Future<void> setValidateCertificate(bool value) async {
    var prefs = await SharedPreferences.getInstance();
    prefs.setBool('validateCertificate', value);
    HttpOverrides.global = value ? null : MyHttpOverrides();
    setState(() {
      _validateCertificate = value;
    });
  }

  @override
  Widget build(BuildContext context) {
    final lang = LanguageController.of(context);
    final isDarkMode = ThemeController.of(context).currentTheme == 'dark';
    LanguageController lc = LanguageController.of(context);

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.settings),
          onPressed: () {
            showAnimatedDialog(
                context: context,
                barrierDismissible: true,
                animationType: DialogTransitionType.slideFromTop,
                curve: Curves.fastOutSlowIn,
                duration: const Duration(milliseconds: 400),
                builder: (context) {
                  return StatefulBuilder(
                    builder: (context, setState) {
                      return AlertDialog(
                        title: Text(lc.getTranslation("settings-title")),
                        content: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            TextField(
                              controller: urlController,
                              decoration: InputDecoration(
                                labelText:
                                    lc.getTranslation("settings-auth-url"),
                              ),
                            ),
                            Row(
                              children: [
                                Text(lc.getTranslation(
                                    "settings-validate-certificate")),
                                const Spacer(),
                                Switch(
                                  value: validateCertificate,
                                  onChanged: (value) {
                                    setValidateCertificate(value)
                                        .then((value) => setState(() {}));
                                  },
                                ),
                              ],
                            ),
                          ],
                        ),
                        actions: [
                          TextButton(
                            onPressed: () {
                              Navigator.of(context).pop();
                            },
                            child: Text(lc.getTranslation("cancel")),
                          ),
                          TextButton(
                            onPressed: () {
                              var url = urlController.text;
                              if (setUrl(url)) {
                                Navigator.of(context).pop();
                              } else {
                                Flushbar(
                                  backgroundColor: Theme.of(context).errorColor,
                                  message: lc.getTranslation("invalid-url"),
                                  duration: const Duration(seconds: 3),
                                ).show(context);
                              }
                            },
                            child: Text(lc.getTranslation("save")),
                          ),
                        ],
                      );
                    },
                  );
                });
          },
        ),
        actions: [
          SizedBox(
            height: double.infinity,
            child: DayNightSwitcherIcon(
              isDarkModeEnabled: isDarkMode,
              // do nothing on state change
              onStateChanged: (_) {
                toggleTheme();
              },
            ),
          ),
          SizedBox(
            height: double.infinity,
            child: ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.transparent,
                elevation: 0,
              ),
              onPressed: () async {
                LanguageController lc = LanguageController.of(context);
                List<Language> languages = await lc.languages;

                // show a dialog to select the language
                showAnimatedDialog(
                  context: context,
                  barrierDismissible: true,
                  animationType: DialogTransitionType.slideFromRightFade,
                  curve: Curves.fastOutSlowIn,
                  duration: const Duration(milliseconds: 400),
                  builder: (context) => AlertDialog(
                    // popup animation
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    backgroundColor:
                        Theme.of(context).cardColor.withOpacity(0.8),
                    title: Text(lang.getTranslation('select-a-language')),
                    content: Column(
                      mainAxisSize: MainAxisSize.min,
                      // map its flag and name
                      // also add a default language
                      children:
                          // default system language
                          [
                                Padding(
                                  padding: const EdgeInsets.all(8.0),
                                  child: ListTile(
                                    title: Text(lc
                                        .getTranslation("system-default-lang")),
                                    onTap: () {
                                      lc
                                          .setPreferSystemLanguage(true)
                                          .then((_) {
                                        Navigator.pop(context);
                                        setState(() {});
                                      });
                                    },
                                  ),
                                )
                              ] +
                              languages
                                  .map(
                                    (e) => Padding(
                                      padding: const EdgeInsets.all(8.0),
                                      child: ListTile(
                                        leading: e.flag,
                                        title: Text(e.name),
                                        onTap: () {
                                          lc.setLanguage(e.code).then((c) => c
                                              .setPreferSystemLanguage(false)
                                              .then((_) => {
                                                    Navigator.pop(context),
                                                    setState(() {})
                                                  }));
                                        },
                                      ),
                                    ),
                                  )
                                  .toList(),
                    ),
                  ),
                );
              },
              child: FutureBuilder(
                future: lang.getFlag,
                builder: (context, snapshot) {
                  if (snapshot.hasData) {
                    return snapshot.data as Widget;
                  }
                  return const CircularProgressIndicator();
                },
              ),
            ),
          ),
        ],
      ),
      body: Stack(children: [
        // wave background
        Positioned(
          top: 0,
          left: 0,
          right: 0,
          child: SizedBox(
            // take all the screen
            height: MediaQuery.of(context).size.height,
            width: MediaQuery.of(context).size.width,
            child: WaveWidget(
              config: CustomConfig(
                // take system colors
                colors: [
                  const Color(0xFF114B5F).withOpacity(0.5),
                  const Color(0xFF456990).withOpacity(0.5),
                  const Color(0xFFE4FDE1).withOpacity(0.5),
                ],
                durations: [35000, 19440, 10800],
                heightPercentages: [0.20, 0.23, 0.25],
              ),
              size: const Size(double.infinity, double.infinity),
              waveAmplitude: 0,
            ),
          ),
        ),
        Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Container(
                  constraints: const BoxConstraints(maxWidth: 500),
                  child: createCard(context, lang)),
            ],
          ),
        ),
      ]),
    );
  }

  Card createCard(BuildContext context, LanguageController lang) {
    return Card(
      color: Theme.of(context).cardColor.withOpacity(0.8),
      elevation: 10,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            // the title
            Text(
              lang.getTranslation('login-title'),
              style: Theme.of(context).textTheme.headline6,
            ),
            // the form
            Form(
              child: FocusTraversalGroup(
                child: Column(
                  children: <Widget>[
                    // the email field
                    TextFormField(
                      textInputAction: TextInputAction.next,
                      controller: usernameController,
                      decoration: InputDecoration(
                        labelText: lang.getTranslation('username-or-email'),
                      ),
                    ),
                    // the password field (with a button to show/hide the password)
                    TextFormField(
                      onFieldSubmitted: (_) => tryLogin(),
                      controller: passwordController,
                      decoration: InputDecoration(
                        labelText: lang.getTranslation('password'),
                        suffixIcon: IconButton(
                          icon: const Icon(Icons.remove_red_eye),
                          onPressed: () {
                            _toggle();
                          },
                        ),
                      ),
                      obscureText: _obscureText,
                    ),
                    Flex(
                      // convert the direction to vertical if the width is less than 600
                      direction: Axis.horizontal,
                      children: <Widget>[
                        // remember me checkbox
                        Row(
                          children: <Widget>[
                            Checkbox(
                              value: _rememberMe,
                              onChanged: (bool? value) {
                                toggleRememberMe();
                              },
                            ),
                            Text(lang.getTranslation('remember-me')),
                          ],
                        ),
                        // forgot password text
                        const Spacer(),
                        TextButton(
                          onPressed: () {},
                          child: Text(
                            lang.getTranslation('forgot-password'),
                          ),
                        ),
                      ],
                    ),

                    Padding(
                      padding: const EdgeInsets.symmetric(vertical: 16.0),
                      child: SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: signingIn ? null : tryLogin,
                          // if is registering, show the register button
                          child: signingIn
                              ? const CircularProgressIndicator()
                              : Text(lang.getTranslation('login')),
                        ),
                      ),
                    ),
                    // register text
                    SizedBox(
                      width: double.infinity,
                      child: TextButton(
                        onPressed: registering ? null : tryRegister,
                        child: registering
                            ? const CircularProgressIndicator()
                            : Text(lang.getTranslation('register')),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void tryLogin() async {
    setSigningIn(true);
    var lang = LanguageController.of(context);
    var urlValue = await url;
    var usernameValue = usernameController.text;
    var passwordValue = passwordController.text;
    try {
      var result = await http.post(Uri.parse("$urlValue/api/Auth"),
          headers: <String, String>{
            'Content-Type': 'application/json; charset=UTF-8',
          },
          body: jsonEncode({
            'id': "",
            'email': usernameValue,
            'password': passwordValue,
          }));
      setSigningIn(false);
      if (!mounted) return;
      if (result.statusCode == 200) {
        var token = jsonDecode(result.body)['token'];
        // show a dialog where the user can copy the token
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: Text(lang.getTranslation('token')),
            content: Text(token),
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.pop(context);
                },
                child: Text(lang.getTranslation('close')),
              ),
              TextButton(
                onPressed: () {
                  Clipboard.setData(ClipboardData(text: token));
                  Navigator.pop(context);
                },
                child: Text(lang.getTranslation('copy')),
              ),
            ],
          ),
        );
      } else {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: result.statusCode == 401
              ? lang.getTranslation("invalid-credentials")
              : lang.getTranslation("unknown-error"),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryLogin();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      }
    } catch (e) {
      // check if is handshake exception
      setSigningIn(false);
      if (!mounted) return;
      if (e is HandshakeException) {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: lang.getTranslation("invalid-certificate", args: {
            "host": urlValue,
          }),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryLogin();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      } else {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: lang.getTranslation("unknown-error"),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryLogin();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      }
    }
  }

  void tryRegister() async {
    setRegistering(true);
    var lang = LanguageController.of(context);
    var urlValue = await url;
    var usernameValue = usernameController.text;
    var passwordValue = passwordController.text;
    try {
      var result = await http.post(Uri.parse("$urlValue/api/Auth/Register"),
          headers: <String, String>{
            'Content-Type': 'application/json; charset=UTF-8',
          },
          body: jsonEncode({
            'id': "",
            'email': usernameValue,
            'password': passwordValue,
          }));
      setRegistering(false);
      if (!mounted) return;
      if (result.statusCode == 200) {
        var token = jsonDecode(result.body)['token'];
        // show a dialog where the user can copy the token
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: Text(lang.getTranslation('token')),
            content: Text(token),
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.pop(context);
                },
                child: Text(lang.getTranslation('close')),
              ),
              TextButton(
                onPressed: () {
                  Clipboard.setData(ClipboardData(text: token));
                  Navigator.pop(context);
                },
                child: Text(lang.getTranslation('copy')),
              ),
            ],
          ),
        );
      } else if (result.statusCode == 401) {
        // unauthorized - already registered
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: lang.getTranslation("email-already-registered",
              args: {"email": usernameValue}),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryRegister();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      } else {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: result.statusCode == 401
              ? lang.getTranslation("invalid-credentials")
              : lang.getTranslation("unknown-error"),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryRegister();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      }
    } catch (e) {
      // check if is handshake exception
      setRegistering(false);
      if (!mounted) return;
      if (e is HandshakeException) {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: lang.getTranslation("invalid-certificate", args: {
            "host": urlValue,
          }),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryRegister();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      } else {
        Flushbar(
          title: lang.getTranslation("error-bar"),
          message: lang.getTranslation("unknown-error"),
          backgroundColor: Theme.of(context).errorColor,
          duration: const Duration(seconds: 3),
          // retry
          mainButton: TextButton(
            // pass itself to the onPressed function
            onPressed: () {
              tryRegister();
              // close the bar
            },
            child: Text(
              lang.getTranslation("retry"),
              style: TextStyle(color: Theme.of(context).primaryColor),
            ),
          ),
        ).show(context);
      }
    }
  }

  Future<void> _loadValidateCertificate() async {
    var prefs = await SharedPreferences.getInstance();
    var validateCertificate = prefs.getBool("validateCertificate");
    if (validateCertificate != null) {
      setValidateCertificate(validateCertificate);
    }
  }

  Future<void> _loadAuthUrl() async {
    setUrl(await url);
  }
}
